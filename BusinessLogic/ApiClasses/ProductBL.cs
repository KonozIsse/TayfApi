using AutoMapper;
using Entities.ViewModel;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Models.Enums;
using Microsoft.CodeAnalysis;
using Entities.Exception;
using Microsoft.Extensions.Configuration;
using Entities.RequestFeatures;
using Twilio.Base;
using System.Security.Policy;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace BusinessLogic.ApiClasses
{
    public class ProductBL
    {
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper;
        private readonly ImageBL _imageBL;
        private readonly Util _util;
        private readonly LocService _locService;
        public ProductBL(IRepositoryManager repositoryManager, IMapper mapper, ImageBL imageBL, Util util , LocService locService)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _imageBL = imageBL;
            _util = util;
            _locService = locService;
        }
        //Category------------------------------------------------
        public async Task<CategoryDto> GetCategory(int catId, string lang)
        {
            var category = await _repositoryManager.Categories.GetCategoryById(catId ,false);
            var categoryDto = _mapper.Map<CategoryDto>(category);
            categoryDto.CategoryName = lang == "en" ? category.CategoryName : category.CategoryNameAr;
            return categoryDto;
        }
        public async Task<PagedList<CategoryDto>> GethMainCategoriesCP(string search, string lang, PostsParameters postsParameters)
        {
            var categories = await _repositoryManager.Categories.SearchMainCategoriesCP(search);
            var mainCategoryDto = categories.Select(category => new CategoryDto
            {
                Id = category.Id,
                CategoryName = lang == "en" ? category.CategoryName : category.CategoryNameAr,
                ImageId = _imageBL.GetImageMedium(category.ImgId),
                IsStatus = category.IsStatus == Status.Active ? _locService.GetLocalizedStringValue("active") : _locService.GetLocalizedStringValue("notActive"),
                CreatedAt = category.CreatedAt.ToString("MM/dd/yyyy hh:mm tt")
            }).ToList();
            return PagedList<CategoryDto>.ToPagedList(mainCategoryDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<PagedList<CategoryDto>> GetSubCategoriesCP(int mainId, string search, string lang, PostsParameters postsParameters)
        {
            var mainCat = await _repositoryManager.Categories.GetCategoryById(mainId, false);
            var categories = await _repositoryManager.Categories.SearchSubCategories(mainId, search);
            var categoriesDto = categories.Select(category => new CategoryDto
            {
                MainCategoryId =mainId ,
                MainCategoryName = lang == "en" ? mainCat.CategoryName : mainCat.CategoryNameAr,
                Id = category.Id,
                CategoryName = lang == "en" ? category.CategoryName : category.CategoryNameAr,
                ImageId = _imageBL.GetImageMedium(category.ImgId),
                IsStatus = category.IsStatus == Status.Active ? _locService.GetLocalizedStringValue("active") : _locService.GetLocalizedStringValue("notActive"),
                CreatedAt = category.CreatedAt.ToString("MM/dd/yyyy hh:mm tt")
            }).ToList();
            return PagedList<CategoryDto>.ToPagedList(categoriesDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<IEnumerable<CategoryDto>> GetAllActiveMainCategories(string lang)
        {
           var categories= await _repositoryManager.Categories.GetAllActiveMainCategories(false);
            var categoriesDto = categories.Select(category => new CategoryDto
            {
                Id = category.Id,
                CategoryName = lang == "en" ? category.CategoryName : category.CategoryNameAr,
                ImageId = _imageBL.GetImageMedium(category.ImgId),
                CreatedAt = category.CreatedAt.ToString("MM/dd/yyyy hh:mm tt")
            }).ToList();
            return categoriesDto;
        }
        public async Task<List<CategoryDto>> GetAllSubActiveCategories(string lang)
        {
            var categories = await _repositoryManager.Categories.GetSubActiveCategories(false);
            var categoriesDto = categories.Select(category => new CategoryDto
            {
                Id = category.Id,
                CategoryName = lang == "en" ? category.CategoryName : category.CategoryNameAr,
                ImageId = _imageBL.GetImageMedium(category.ImgId),
                CreatedAt = category.CreatedAt.ToString("MM/dd/yyyy hh:mm tt")
            }).ToList();
            return categoriesDto;
        }
        public async Task<PagedList<CategoryDto>> GetActiveSubCategoriesMainId(int mainId,string lang, PostsParameters postsParameters)
        {
            var mainCat = await _repositoryManager.Categories.GetCategoryById(mainId, false);
            var categories = await _repositoryManager.Categories.GetSubCategoriesByMainId(mainId, false);
            var categoriesDto = categories.Select(category => new CategoryDto
            {
                MainCategoryId = mainId,
                MainCategoryName = lang == "en" ? mainCat.CategoryName : mainCat.CategoryNameAr,
                Id = category.Id,
                CategoryName = lang == "en" ? category.CategoryName : category.CategoryNameAr,
                ImageId = _imageBL.GetImageOriginal(category.ImgId),
                CreatedAt = category.CreatedAt.ToString("MM/dd/yyyy hh:mm tt")
            }).ToList();
            return PagedList<CategoryDto>.ToPagedList(categoriesDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<BussnessResultModel> CreateCategory(CreateCategoryDto create)
        {
            var category = _mapper.Map<Category>(create);
            if (create.ImgId == 0)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctImage"),false); 
            }

            if (create.CategoryNameAr == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterallfiled"),false); 
            }
            if (create.MainCategoryId != null)
            {
                category.MainCategoryId = create.MainCategoryId.Value;
            }
            else
            {
                category.MainCategoryId = 0;
            }
            _repositoryManager.Categories.CreateMainCategory(category);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(category, _locService.GetLocalizedStringValue("successAdd"));
        }
        public async Task<BussnessResultModel> DeleteCategory(int id)
        {
            var product = _repositoryManager.Product.GetProductsCatId (id);
            if(product != null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("Delete_Product_Linked_With_Category_First"), false);
            }
            var MainCategory = await _repositoryManager.Categories.GetCategoryById(id, false);
            if (MainCategory == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            else
            {
                var subCategoryList = await _repositoryManager.Categories.GetSubCategoriesByMainId(id, false);
                if (subCategoryList != null)
                {
                    foreach (var item in subCategoryList)
                    {
                        _repositoryManager.Categories.DeleteCategory(item);
                    }
                }
                _repositoryManager.Categories.DeleteCategory(MainCategory);
            }  
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(MainCategory, _locService.GetLocalizedStringValue("successDelete"));
        }
        public async Task<BussnessResultModel> EditCategory (UpdateCategoryDto updateDto)
        {
            var category = await _repositoryManager.Categories.GetCategoryById(updateDto.Id, true);
            if(category == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"),false);
            }
            else
            {
                if (updateDto.CategoryNameAr == null)
                {
                    return new BussnessResultModel(category, _locService.GetLocalizedStringValue("enterallfiled"), false);
                }
                if (updateDto.MainCategoryId == null)
                {
                    category.MainCategoryId = 0;
                }
                else
                {
                    category.MainCategoryId = updateDto.MainCategoryId.Value;
                }
                _mapper.Map(updateDto, category);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(category, _locService.GetLocalizedStringValue("successSave"));
            }
        }

        //Product------------------------------------------------
        public async Task<BussnessResultModel> AddProduct(int userId , CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);
            product.StoreId = createProductDto.StoreId;
            if (createProductDto.DescriptionAr == null || createProductDto.ProductNameAr == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterallfiled"), false);
            }
            if (createProductDto.Price < 0)
            {
                return new BussnessResultModel(product, _locService.GetLocalizedStringValue("enterPrice"), false);
            }
            var user = await _repositoryManager.User.GetActiveUserId(userId, false);
            if(user.UserType == UserType.Admin)
            {
                product.AdminId = userId;
                product.IsAcceptAdmin = true;
            }
            else
            {
                product.IsAcceptAdmin = false;
            }
            if (createProductDto.ProductCategories != null)
            {
                product.ProductCategories = new List<ProductCategory>();
                product.ProductCategories.AddRange(_mapper.Map<List<ProductCategory>>(createProductDto.ProductCategories));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("selectcategory"), false);
            }
            if (createProductDto.ImagesProduct != null)
            {
                product.Images = new List<ProductImage>();
                product.Images.AddRange(_mapper.Map<List<ProductImage>>(createProductDto.ImagesProduct));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctImage"), false);
            }
            if (createProductDto.IsSale == true)
            {
                product.ProductSales = new List<ProductSales>();
                var sale = createProductDto.ProductSales.First();
                if (sale.StartDate > sale.EndDate)
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("StartDateMustBeGaraterThanExpireDate"), false);
                }
                product.ProductSales.AddRange(_mapper.Map<List<ProductSales>>(createProductDto.ProductSales));
            }
            if (createProductDto.IsSpecial == true)
            {
                product.SpecialProducts = new List<SpecialProducts>();
                if (_util.EasternTime > createProductDto.SpecialProducts.First().EndDate)
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("StartDateMustBeGaraterThanExpireDate"), false);
                }
                product.SpecialProducts.AddRange(_mapper.Map<List<SpecialProducts>>(createProductDto.SpecialProducts));
            }
            _repositoryManager.Product.AddProduct(product);
            await _repositoryManager.SaveAsync();
            if (createProductDto.Availability != 0)
            {
                var inventory = new Inventory
                {
                    Stock = createProductDto.Availability,
                    ProductId = product.Id,
                    StockType = "in",
                    AddedDate = _util.EasternTime.Millisecond,
                    VendorId =  product.StoreId,
                    AdminId = user.UserType == UserType.Admin ? userId : null,
                };
               _repositoryManager.Inventory.AddInventory(inventory);
                await _repositoryManager.SaveAsync();
            }
            return new BussnessResultModel(product , _locService.GetLocalizedStringValue("successAdd"));
        }
        public async Task<BussnessResultModel> EditProduct(int userId, UpdateProductDto updateDto)
        {
            var product = await _repositoryManager.Product.GetProductById(updateDto.Id, true);
            if(product == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            var user = await _repositoryManager.User.GetActiveUserId(userId, false);
            if (user.UserType == UserType.Admin)
            {
                product.AdminId = userId;
            }
            if (updateDto.DescriptionAr == null || updateDto.ProductNameAr == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterallfiled"), false);
            }
            if (updateDto.Price < 0)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterPrice"), false);
            }
            if (updateDto.ProductCategories != null)
            {
                var categoriesProdId = await _repositoryManager.ProductCategory.GetAllCategoriesProductId(product.Id, false);
                if (categoriesProdId.Count() > 0 && categoriesProdId != null)
                {
                    foreach (var category in categoriesProdId)
                    {
                        _repositoryManager.ProductCategory.DeleteProductCategory(category);
                    }
                }
                product.ProductCategories.AddRange(_mapper.Map<List<ProductCategory>>(updateDto.ProductCategories));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("selectcategory"), false);
            }
            if (updateDto.ImagesProduct != null)
            {
                var categoriesProdId = await _repositoryManager.ImageProduct.GetAllImagesProduct(product.Id, false);
                if (categoriesProdId.Count() > 0 && categoriesProdId != null)
                {
                    foreach (var category in categoriesProdId)
                    {
                        _repositoryManager.ImageProduct.DeleteImageProduct(category);
                    }
                }
                product.Images.AddRange(_mapper.Map<List<ProductImage>>(updateDto.ImagesProduct));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctImage"), false);
            }
            _mapper.Map(updateDto, product);
            await _repositoryManager.SaveAsync();
            if (updateDto.IsSale == true )
            {
                var sale = updateDto.ProductSales.First();
                if (sale.StartDate > sale.EndDate)
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("StartDateMustBeGaraterThanExpireDate"),false);
                }
                var postSeePeopleDto = updateDto.ProductSales.Select(x => x.Id);
                var sales = await _repositoryManager.Sales.GetAllSalesProductId(updateDto.Id, true);
                var Ids = sales.Select(x => x.Id);
                var listToDelete = Ids.Except(postSeePeopleDto).ToList();
                await _repositoryManager.Sales.DeleteListSales(listToDelete);
                var listToAdd = updateDto.ProductSales.Where(x => x.Id == 0);
                var entity = _mapper.Map<List<ProductSales>>(listToAdd);
                foreach (var item in entity)
                {
                    item.ProductId = product.Id;
                }
                _repositoryManager.Sales.CreateListSales(entity);
                await _repositoryManager.SaveAsync();
            }
            else 
            {
                var salesProduct = await _repositoryManager.Sales.CheckFlashExists(updateDto.Id, false);
                if (salesProduct != null)
                {
                    _repositoryManager.Sales.DeleteFlashSale(salesProduct);
                    await _repositoryManager.SaveAsync();
                }
            }
            if (updateDto.IsSpecial == true )
            {
                if (_util.EasternTime > updateDto.SpecialProducts.First().EndDate)
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("StartDateMustBeGaraterThanExpireDate"), false); 
                }
                var postSeePeopleDto = updateDto.SpecialProducts.Select(x => x.Id);
                var sales = await _repositoryManager.SpecialProducts.GetSpecialProductsProductId(updateDto.Id, true);
                var Ids = sales.Select(x => x.Id);
                var listToDelete = Ids.Except(postSeePeopleDto).ToList();
                await _repositoryManager.SpecialProducts.DeleteListSpecialProducts(listToDelete);
                var listToAdd = updateDto.SpecialProducts.Where(x => x.Id == 0);
                var entity = _mapper.Map<List<SpecialProducts>>(listToAdd);
                foreach (var item in entity)
                {
                    item.ProductId = product.Id;
                }
                _repositoryManager.SpecialProducts.CreateListSpecialProducts(entity);
                
                await _repositoryManager.SaveAsync();
            }
            else
            {
                var special = await _repositoryManager.SpecialProducts.CheckSpecialExists(updateDto.Id,false);
                if (special != null)
                {
                    _repositoryManager.SpecialProducts.DeleteSpecialProduct(special);
                    await _repositoryManager.SaveAsync();
                }
            }
            return new BussnessResultModel(product, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<BussnessResultModel> ApproveProduct(int productId)
        {
            var product = await _repositoryManager.Product.GetProductById(productId, true);
            if (product == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            var exsit = await _repositoryManager.Product.CheckApproveProduct(productId);
            if (exsit != null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("alreadyApproved"), false);
            }
            product.IsAcceptAdmin = true;
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(product, _locService.GetLocalizedStringValue("approveProduct"));
        }
        public async Task<BussnessResultModel> RemoveProduct(int productId)
        {
            var orders = await _repositoryManager.OrderProducts.GetOrdersProductId(productId);
            if (orders.Count > 0)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("NotDeleteProductInOrder"),false);
            }
            var product = await _repositoryManager.Product.GetProductById(productId, false);
            if (product != null )
            {
                var specials = await _repositoryManager.SpecialProducts.GetSpecialProductsProductId(productId,false);
                if (specials != null)
                {
                    foreach (var special in specials)
                    {
                        _repositoryManager.SpecialProducts.DeleteSpecialProduct(special);
                    }
                }
                var sales = await _repositoryManager.Sales.GetAllSalesProductId(productId,false);
                if (sales != null)
                {
                    foreach (var sale in sales)
                    {
                        _repositoryManager.Sales.DeleteFlashSale(sale);
                    }
                }
                var likes = await _repositoryManager.WishList.GetLikesProductId(productId);
                if (likes != null)
                {
                    foreach (var like in likes)
                    {
                        _repositoryManager.WishList.DeleteLike(like);
                    }
                }
                var reviews = await _repositoryManager.Review.GetReviewsProductId(productId);
                if (reviews != null)
                {
                    foreach (var review in reviews)
                    {
                        _repositoryManager.Review.DeleteReview(review);
                    }
                }
                var attributs = await _repositoryManager.Attribute.GetAttributesProductId(productId);
                if (attributs != null)
                {
                    foreach (var attribut in attributs)
                    {
                        _repositoryManager.Attribute.DeleteAttributesProduct(attribut);
                    }
                }
                var inventories = await _repositoryManager.Inventory.GetAllInventoryByPrductId(productId);
                if (inventories != null)
                {
                    foreach (var inventory in inventories)
                    {
                        _repositoryManager.Inventory.DeleteInventory(inventory);
                    }
                }

                _repositoryManager.Product.DeleteProduct(product);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(orders, _locService.GetLocalizedStringValue("successDelete"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"),false);
            }
           
        }
        public async Task<List<ProductDto>> GetAllActiveAcceptProducts(int? customerId, string lang)
        {
            var products = await _repositoryManager.Product.GetAllAcceptedProducts();
            var productsDto = new List<ProductDto>();
            
            if (products != null)
            {
                foreach (var product in products)
                {
                    var special = await _repositoryManager.SpecialProducts.GetSpecialProductId(product.Id);
                    var flash = await _repositoryManager.Sales.GetFlashProductId(product.Id);
                  
                        var cats = await _repositoryManager.ProductCategory.GetAllCategoriesProductId(product.Id, false,true);
                        var catsDto = cats.Select(c => new ProductCategoryDto
                        {
                            MainCategoryId = c.Category.MainCategoryId,
                            CategoryId = c.CategoryId,
                            CategoryName = lang == "en" ? c.Category.CategoryName : c.Category.CategoryNameAr,
                            CategoryImage = _imageBL.GetImageThumbnail(c.Category.ImgId),
                        }).ToList();
                        productsDto.Add(new ProductDto
                        {
                          ProductCategories = catsDto,
                            Id = product.Id,
                            ProductName = lang == "en" ? product.ProductName : product.ProductNameAr,
                            Description = lang == "en" ? product.Description : product.DescriptionAr,
                            ProductModel = product.ProductModel,
                            ImageProduct = _imageBL.GetImageThumbnail(product.Images.First().ImageId),
                            TypeId = product.TypeId,
                            Price = product.Price,
                            IsStatus = product.IsStatus.ToString(),
                            Availability =  AvailabilityProducts(product.Id),
                            AttributesProducts = await GetAttributsProducts(product.Id),
                            Images = await _imageBL.GetAllImagesProductId(product.Id),
                            ShareLink = _util.url1 + "/share.html?id=" + product.Id,
                            IsBest = product.IsBest,
                            IsFeature = product.IsFeature,

                            IsSpecial = product.IsSpecial,
                            SpecialPrice = product.IsSpecial == false ? 0 : special.SpecialPrice,
                            EndDateSpecial = (special != null ? special.EndDate : null),

                            IsSale = product.IsSale,
                            DiscountPrice = (flash != null ? flash.DiscountPrice : 0), 
                            StartDate = (flash != null ? flash.StartDate : null),
                            EndDate = (flash != null ? flash.EndDate : null),


                            IsFavorite =   await IsFavourite(customerId.Value, product.Id),
                            NumLike = await GetFavourite(customerId.Value, product.Id),
                            Reviews = await GetActiveReviews(product.Id),
                            Rate = await Rate(product.Id),

                            StoreId = product.Store == null ? 0 : product.StoreId,
                            StoreName = product.Store != null ?  product.Store.FullName : null,
                            StoreImage = product.Store == null ? null : product.Store.Avater
                            
                        }) ; 
                    
                }
                foreach (var item in productsDto)
                {
                    if (item.IsSale == true && products.First().ProductSales  != null)
                    {
                        item.Price = item.DiscountPrice;
                    }
                    if (item.IsSpecial == true && products.First().SpecialProducts != null)
                    {
                        item.Price = item.SpecialPrice;
                    }
                }
                return productsDto;
            }
            else
            {
                return new List<ProductDto>();
            }
        }
        public async Task<PagedList<ProductDto>> GetProductsCP(int userId , string search , string lang, PostsParameters postsParameters)
        {
            var products = await _repositoryManager.Product.GetProductsCP(search);
            //var productsDto =_mapper.Map<IEnumerable<ProductDto>>(products);
            var store = await _repositoryManager.User.GetUserId(userId, false);
            if (store.UserType == UserType.Store)
            {
                products = products.Where(c => c.StoreId == userId).ToList();
            }
            var productsDto = new List<ProductDto>();
            foreach (var item in products)
            {
                var cats = await _repositoryManager.ProductCategory.GetAllCategoriesProductId(item.Id,false,true);
               
                    var catsDto = cats.Select( c => new ProductCategoryDto
                    {
                        MainCategoryId = c.Category.MainCategoryId,
                        CategoryId = c.CategoryId,
                        CategoryName = lang == "en" ? c.Category.CategoryName : c.Category.CategoryNameAr,
                        CategoryImage =  _imageBL.GetImageThumbnail(c.Category.ImgId),
                    }).ToList();
                var images = await _repositoryManager.ImageProduct.GetAllImagesProduct(item.Id,false,true);
                productsDto.Add(new ProductDto
                {
                    Id = item.Id,
                    ProductName = lang == "en" ? item.ProductName : item.ProductNameAr,
                    Description = lang == "en" ? item.Description : item.DescriptionAr,
                    IsStatus = item.IsStatus.ToString(),
                    ImageProduct = _imageBL.GetImageThumbnail(images.First().ImageId) ?? null,
                    Availability = AvailabilityProducts(item.Id),
                    NumLike = item.WishLists.Count(),
                    ProductCategories = catsDto
                });
            }
            return PagedList<ProductDto>.ToPagedList(productsDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<ProductDto> GetProductDetails(int productId, int customerId, string lang)
        {
            var product = await _repositoryManager.Product.GetAcceptAdminActiveProduct(productId);
            if (product == null) { return null; }

            var special = await _repositoryManager.SpecialProducts.GetSpecialProductId(productId);
            var flash = await _repositoryManager.Sales.GetFlashProductId(product.Id);
            
            var name = lang == "en" ? product.ProductName : product.ProductNameAr;
            var descreption = lang == "en" ? product.Description : product.DescriptionAr;

            var cats = await _repositoryManager.ProductCategory.GetAllCategoriesProductId(product.Id, false,true);
            var catsDto = cats.Select(c => new ProductCategoryDto
            {
                MainCategoryId = c.Category.MainCategoryId,
                CategoryId = c.CategoryId,
                CategoryName = lang == "en" ? c.Category.CategoryName : c.Category.CategoryNameAr,
                CategoryImage = _imageBL.GetImageMedium(c.Category.ImgId),
            }).ToList() ;
            var images = await _repositoryManager.ImageProduct.GetAllImagesProduct(productId, false, true);
            return new ProductDto
            {
                ProductCategories = catsDto,
                Id = product.Id,
                ProductName = name,
                Description = descreption,
                ProductModel = product.ProductModel,
                ImageProduct = _imageBL.GetImageOriginal(images.First().ImageId) ?? null,
                TypeId = product.TypeId,
                Price = product.Price,
                IsStatus = product.IsStatus.ToString(),
                Availability =  AvailabilityProducts(product.Id) ,
                AttributesProducts = await GetAttributsProducts(product.Id) ?? null,
                Images = await _imageBL.GetAllImagesProductId(product.Id) ?? null,
                ShareLink = _util.url1 + "/en/Home/share?id=" + product.Id + "&name=" + name.Trim(),
                IsBest = product.IsBest,
                IsFeature = product.IsFeature,

                IsSpecial = product.IsSpecial,
                SpecialPrice = product.IsSpecial == false ? 0 : special.SpecialPrice,

                IsSale = product.IsSale,
                DiscountPrice = (flash != null ? flash.DiscountPrice : 0),
                StartDate = (flash != null ? flash.StartDate : null),
                EndDate = (flash != null ? flash.EndDate : null),

                IsFavorite = await IsFavourite(customerId, product.Id) ,
                NumLike = await GetFavourite(customerId, product.Id) ,
                Reviews = await GetActiveReviews(product.Id)??null,
                Rate = await Rate(product.Id) == 0 ? 0 : await Rate(product.Id),

                StoreId = product.StoreId == null ? 0 : product.StoreId,
                StoreName =   product.Store.FullName ?? null,
                StoreImage =  _imageBL.GetImageMedium(product.Store.ImageId.Value) ?? null
            };
        }
        public async Task<CatStoreProductVM> GetAllProducts(int? catId, int customerId, string lang ,int type, int? price1, int? price2)
        {
            var categories = await GetAllSubActiveCategories(lang);
            var products = await GetAllActiveAcceptProducts(customerId, lang);
            if(catId != null)
            {
                products = products.Where(c => c.ProductCategories.Any(c => c.CategoryId == catId)).ToList();
            }
           
            if (price2 != 0 && price2 < 1000)
            {
                products = products.Where(r => (!r.IsSpecial && r.Price >= price1 && r.Price <= price2)
                || (r.IsSpecial && r.SpecialPrice >= price1 && r.SpecialPrice <= price2)).ToList();
            }
            if (price2 >= 1000)
            {
                products = products.Where(r => (!r.IsSpecial && r.Price >= price1) || (r.IsSpecial && r.SpecialPrice >= price1)).ToList();
            }
            var stores = new List<StoreDto>();
            if (type == 1)
            {
                var productsStore = products.GroupBy(x => x.StoreId).Select(x => x.First()).ToList();
                foreach (var store in productsStore)
                {
                    var storeDB = await _repositoryManager.User.GetStoreId(store.StoreId);
                    if (storeDB != null)
                    {
                        stores.Add(new StoreDto
                        {
                            Id = storeDB.Id,
                            FirstName = storeDB.FirstName,
                            Image = _imageBL.GetImageOriginal(storeDB.ImageId.Value),
                            AdressInfo = storeDB.AdressInfo
                        });
                    }
                }
            }
            var model = new CatStoreProductVM
            {
                Products = products,
                Categories = categories,
                Stores = stores
            };
            return model;
        }
        public async Task<CatStoreProductVM> GetAllSearchProducts(int? catId, int customerId,  string search, string lang, int PageId , int? sort, int type , int? price1, int? price2, int RowsPerPage = 15)
        {
            var categories = await GetAllSubActiveCategories(lang);
            var products = await GetAllActiveAcceptProducts(customerId, lang);
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(c => c.ProductName.Contains(search)  || c.Description.Contains(search) || c.ProductCategories.Any(x=>x.CategoryName.Contains(search))).ToList();
            }
            if (catId != null)
            {
                products = products.Where(x => x.Availability > 0 && x.ProductCategories.Any(c => c.CategoryId == catId || c.MainCategoryId == catId)).ToList();
            }
            if (price2 != 0 && price2 < 1000)
            {
                products = products.Where(r => r.Availability > 0 && (!r.IsSpecial && r.Price >= price1 && r.Price <= price2)
                || (r.IsSpecial && r.SpecialPrice >= price1 && r.SpecialPrice <= price2)).ToList();
            }
            if (price2 >= 1000)
            {
                products = products.Where(r => r.Availability > 0 && (!r.IsSpecial && r.Price >= price1) || (r.IsSpecial && r.SpecialPrice >= price1)).ToList();
            }
            int TotalProductCount = products.Count();
            int PageCount = (int)Math.Ceiling((float)TotalProductCount / RowsPerPage);
            if (PageId < 1 || PageId > PageCount)
                PageId = 1;
            int SkipCount = RowsPerPage * (PageId - 1);

            var items = products.OrderByDescending(x => x.CreatedAt).Skip(SkipCount).Take(RowsPerPage).ToList();
            if (sort == 1)
            {
                items = products.OrderBy(x => x.ProductName).Skip(SkipCount).Take(RowsPerPage).ToList();
            }
            else if (sort == 2)
            {
                items = products.OrderByDescending(x => x.ProductName).Skip(SkipCount).Take(RowsPerPage).ToList();
            }
            if (sort == 3)
            {
                items = products.OrderBy(x => x.Price).Skip(SkipCount).Take(RowsPerPage).ToList();
            }
            else if (sort == 4)
            {
                items = products.OrderByDescending(x => x.Price).Skip(SkipCount).Take(RowsPerPage).ToList();
            }
            if (sort == 5)
            {
                items = products.OrderBy(x => x.Rate).Skip(SkipCount).Take(RowsPerPage).ToList();
            }
            else if (sort == 6)
            {
                items = products.OrderByDescending(x => x.Rate).Skip(SkipCount).Take(RowsPerPage).ToList();
            }
            if (sort == 7)
            {
                items = products.OrderBy(x => x.ProductModel).Skip(SkipCount).Take(RowsPerPage).ToList();
            }
            else if (sort == 8)
            {
                items = products.OrderByDescending(x => x.ProductModel).Skip(SkipCount).Take(RowsPerPage).ToList();
            }
            var stores = new List<StoreDto>();
            if (type == 1)
            {
                var productsStore = products.GroupBy(x => x.StoreId).Select(x => x.First()).ToList();
                foreach (var store in productsStore)
                {
                    var storeDB = await _repositoryManager.User.GetStoreId(store.StoreId);
                    if (storeDB != null)
                    {
                        stores.Add(new StoreDto
                        {
                            Id = storeDB.Id,
                            FirstName = storeDB.FirstName,
                            Image = _imageBL.GetImageOriginal(storeDB.ImageId.Value),
                            AdressInfo = storeDB.AdressInfo
                        });
                    }
                }
            }
            var model = new CatStoreProductVM
            {
                Products = items,
                Categories = categories,
                Stores = stores
            };
            return model;
        }
        public async Task<List<ProductDto>> GetMyWishList(int? catId, int customerId, string lang, int? sort, int type, int? price1, int? price2)
        {
            var products = await GetAllProducts(catId ,customerId, lang, type, price1,price2);
            var productsList = products.Products.Where(c => c.IsFavorite == true).ToList();
            if (sort == 1)
            {
                productsList = productsList.OrderBy(x => x.ProductName).ToList();
            }
            else if (sort == 2)
            {
                productsList = productsList.OrderByDescending(x => x.ProductName).ToList();
            }
            if (sort == 3)
            {
                productsList = productsList.OrderBy(x => x.Price).ToList();
            }
            else if (sort == 4)
            {
                productsList = productsList.OrderByDescending(x => x.Price).ToList();
            }
            if (sort == 5)
            {
                productsList = productsList.OrderBy(x => x.Rate).ToList();
            }
            else if (sort == 6)
            {
                productsList = productsList.OrderByDescending(x => x.Rate).ToList();
            }
            if (sort == 7)
            {
                productsList = productsList.OrderBy(x => x.ProductModel).ToList();
            }
            else if (sort == 8)
            {
                productsList = productsList.OrderByDescending(x => x.ProductModel).ToList();
            }
            return productsList;
        }
        public async Task<List<ProductDto>> GetAllProductsCategory(int catId ,int customerId ,string lang)
        {
            var products = await GetAllActiveAcceptProducts(customerId, lang);
            var productsCatId = products.Where(c => c.ProductCategories.Any(x=> x.CategoryId == catId)).ToList();
            return productsCatId;
        }
        public async Task<PagedList<ProductDto>> GetAllProductsToCategory(int catId, int customerId, string lang, int? sort , PostsParameters postsParameters)
        {
            var products = await GetAllActiveAcceptProducts(customerId, lang);
            var productsList = products.Where(c => c.ProductCategories.Any(x => x.CategoryId == catId)).ToList();
            if (sort == 1)
            {
                productsList = productsList.OrderBy(x => x.ProductName).ToList();
            }
            else if (sort == 2)
            {
                productsList = productsList.OrderByDescending(x => x.ProductName).ToList();
            }
            if (sort == 3)
            {
                productsList = productsList.OrderBy(x => x.Price).ToList();
            }
            else if (sort == 4)
            {
                productsList = productsList.OrderByDescending(x => x.Price).ToList();
            }
            if (sort == 5)
            {
                productsList = productsList.OrderBy(x => x.Rate).ToList();
            }
            else if (sort == 6)
            {
                productsList = productsList.OrderByDescending(x => x.Rate).ToList();
            }
            if (sort == 7)
            {
                productsList = productsList.OrderBy(x => x.ProductModel).ToList();
            }
            else if (sort == 8)
            {
                productsList = productsList.OrderByDescending(x => x.ProductModel).ToList();
            }
            return PagedList<ProductDto>.ToPagedList(productsList, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<CatStoreProductVM> GetAllProductsToStore(int storeId,int? catId, int customerId, string lang, int type, int? price1, int? price2)
        {
            var categories = await GetAllSubActiveCategories(lang);
            var products = await GetAllActiveAcceptProducts(customerId, lang);
            products = products.Where(c => c.StoreId == storeId).ToList();
            //-------------------------------------------------
            var store = await _repositoryManager.User.GetStoreId(storeId);
            var storeDto = _mapper.Map<StoreDto>(store);
            storeDto.Image = _imageBL.GetImageMedium(store.ImageId.Value);
            //-------------------------------------------------
            if (catId != null)
            {
                products = products.Where(c => c.ProductCategories.Any(c => c.CategoryId == catId)).ToList();
            }

            if (price2 != 0 && price2 < 1000)
            {
                products = products.Where(r => (!r.IsSpecial && r.Price >= price1 && r.Price <= price2)
                || (r.IsSpecial && r.SpecialPrice >= price1 && r.SpecialPrice <= price2)).ToList();
            }
            if (price2 >= 1000)
            {
                products = products.Where(r => (!r.IsSpecial && r.Price >= price1) || (r.IsSpecial && r.SpecialPrice >= price1)).ToList();
            }
            var stores = new List<StoreDto>();
            if (type == 1)
            {
                var productsStore = products.GroupBy(x => x.StoreId).Select(x => x.First()).ToList();
                foreach (var item in productsStore)
                {
                    var storeDB = await _repositoryManager.User.GetStoreId(item.StoreId);
                    if (storeDB != null)
                    {
                        stores.Add(new StoreDto
                        {
                            Id = storeDB.Id,
                            FirstName = storeDB.FirstName,
                            Image = _imageBL.GetImageOriginal(storeDB.ImageId.Value),
                            AdressInfo = storeDB.AdressInfo
                        });
                    }
                }
            }
            var model = new CatStoreProductVM
            {
                Products = products,
                Categories = categories,
                Stores = stores,
                Store = storeDto
            };
            return model;
        }
        public async Task<List<ProductDto>> GetSpecialsProd(int customerId,string lang)
        {
            var products = await GetAllActiveAcceptProducts(customerId, lang);
             products = products.Where(c=>c.IsSpecial == true && c.EndDateSpecial < _util.EasternTime).ToList();
            return products;
        }
        public async Task<List<ProductDto>> GetFlashProds(int customerId,string lang)
        {
            var products = await GetAllActiveAcceptProducts(customerId, lang);
             products = products.Where(c=>c.IsSale == true && c.EndDate < _util.EasternTime).ToList();
            return products;
        }
        public async Task<List<ProductDto>> PopularsPage(int pageSize = 10)
        {
            var popular = await _repositoryManager.Product.GetPopularProducts(pageSize);
            var popularDto = _mapper.Map<List<ProductDto>>(popular);
            return popularDto;
        }
        public async Task<List<ProductDto>> BestPage(int pageSize = 10)
        {
            var popular = await _repositoryManager.Product.GetBestProducts(pageSize);
            var popularDto = _mapper.Map<List<ProductDto>>(popular);
            return popularDto;
        }
        public async Task<List<ProductDto>> LatestPage(int pageSize = 10)
        {
            var populars = await _repositoryManager.Product.GetLatestPage(pageSize);
            var popularsDto = _mapper.Map<List<ProductDto>>(populars);
            return popularsDto;
        }
        public async Task<List<ProductDto>> SpecialsPage(int pageSize = 5)
        {
            var populars = await _repositoryManager.Product.SpecialsPage(pageSize);
            var popularsDto = _mapper.Map<List<ProductDto>>(populars);
            return popularsDto;
        }
        public async Task<List<ProductDto>> TopRatedPage(int pageSize = 6)
        {
            var populars = await _repositoryManager.Product.TopRatedPage(pageSize);
            var popularsDto = _mapper.Map<List<ProductDto>>(populars);
            return popularsDto;
        }
        public async Task<List<ProductDto>> DailyDeals()
        {
            var populars = await _repositoryManager.Product.DailyDeals();
            var popularsDto = _mapper.Map<List<ProductDto>>(populars);
            return popularsDto;
        }
        
        //ProductType------------------------------------------------
        public async Task<List<string>> GetProductTypeEnum(string lang)
        {
            var names = Enum.GetNames(typeof(ProductsType)).ToList();
            foreach (string type in names)
            {
                if ( type == ProductsType.Simple.ToString())
                {
                    type.Equals ( lang == "en" ? ProductsType.Simple.ToString() : " منتج بسيط");
                }
                else if (type == ProductsType.Variable.ToString())
                {
                    type.Equals(lang == "en" ? ProductsType.Variable.ToString() : " منتج له سمات");
                }
                else
                {
                    type.Equals(lang == "en" ? ProductsType.External.ToString() : " منتج خارجي");
                }
            }
            return names;
        }
       
        public async Task<List<ProductType>> GetProductTypes(string lang)
        {
            var list = new List<ProductType>();
            var types = await _repositoryManager.ProductType.GetProductTypes();
            foreach (var type in types)
            {
                if (type.Id == 1)
                { type.Type = lang == "en" ? type.Type : " منتج بسيط"; }
                else if (type.Id == 2)
                { type.Type = lang == "en" ? type.Type : " منتج له سمات"; }
                else 
                { type.Type = lang == "en" ? type.Type : " منتج خارجي"; }

                list.Add(new ProductType
                {
                    Id = type.Id,
                    Type = type.Type
                });
            }
            return list;
        }
        //AttributesProduct------------------------------------------------
        public async Task<List<AttributeDto>> GetAttributsProducts(int productId)
        {
            var attributs = await _repositoryManager.Attribute.GetAttributesProductId(productId);
            var attributsDto = _mapper.Map<List<AttributeDto>>(attributs);
            return attributsDto;
        }
        public async Task<BussnessResultModel> AddAttribute (int productId ,CreateAttributeDto createDto)
        {
            if (createDto.OptionId != 0 && createDto.ValueId != 0)
            {
                var productOptionValue = await _repositoryManager.Attribute.GetProductOptionValue(productId, createDto.OptionId, createDto.ValueId);
                if (productOptionValue == null)
                {
                    var attribute = _mapper.Map<ProductAttribut>(createDto);
                    attribute.ProductId = productId;
                    if (createDto.IsDefault == 1)
                    {
                        attribute.AttributePrice = 0;
                        attribute.PricePrefix = "+";
                    }
                    else
                    {
                        attribute.AttributePrice = createDto.AttributePrice;
                    }
                    _repositoryManager.Attribute.AddAttributesProduct(productId, attribute);
                    await _repositoryManager.SaveAsync();
                    return new BussnessResultModel(attribute, _locService.GetLocalizedStringValue("successAdd"));
                }
                else
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("ExistItem"), false);
                }
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("Error"), false);
            }
        }
        public async Task<BussnessResultModel> DeleteAttribute(int id)
        {
            var attribut = await _repositoryManager.Attribute.GetAttributeId(id, false);
            if (attribut == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            _repositoryManager.Attribute.DeleteAttributesProduct(attribut);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(attribut, _locService.GetLocalizedStringValue("successDelete"));
        }
        public async Task<BussnessResultModel> UpdateAttribute(UpdateAttributeDto update)
        {
            var attribut = await _repositoryManager.Attribute.GetAttributeId(update.Id, true);
            if(attribut != null)
            {
                if (update.AttributePrice < 0)
                {
                    return new BussnessResultModel(attribut, _locService.GetLocalizedStringValue("enterPrice"),false); 
                }
                _mapper.Map(update, attribut);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(attribut, _locService.GetLocalizedStringValue("successSave"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"),false);
            }
        }
        public async Task<string> GetPriceForAttribute(int id)
        {
            decimal productPrice = 0;
            int stock = 0;
            var attrProduct = await _repositoryManager.Attribute.GetAttributeId(id, false);
            if (attrProduct != null)
            {
                int prodId = attrProduct.ProductId;
                var product = await _repositoryManager.Product.GetProductById(prodId, false);
                if (product != null)
                {
                    productPrice = product.Price;
                }
                var special = await _repositoryManager.SpecialProducts.GetSpecialProductId(prodId);
                if (special != null)
                {
                    productPrice = special.SpecialPrice;
                }
                var flashSale = await _repositoryManager.Sales.GetFlashProductId(prodId);
                if (flashSale != null)
                {
                    productPrice = flashSale.DiscountPrice;
                }
                if (attrProduct.PricePrefix == "+")
                {
                    productPrice += attrProduct.AttributePrice;
                }
                else
                {
                    if (productPrice != 0)
                    {
                        productPrice -= attrProduct.AttributePrice;
                    }
                }
                var inStockList = await _repositoryManager.Inventory.GetProductIdOptoinIdInStock(prodId, id);
                var OutStockList = await _repositoryManager.Inventory.GetProductIdOptoinIdOutStock(prodId, id);
                stock = inStockList.Sum(r => r.Stock) - OutStockList.Sum(r => r.Stock);
            }
            return productPrice + "  _  " + stock;
        }
        //Option------------------------------------------------
        public async Task<BussnessResultModel> AddOption(CreateOptionDto createOptionDto)
        {
            var option = _mapper.Map<ProductOption>(createOptionDto);
            if (createOptionDto.OptionName == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterallfiled"),false);
            }
            _repositoryManager.Option.CreateOption(option);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(option, _locService.GetLocalizedStringValue("successAdd"));
        }
        public async Task<BussnessResultModel> EditOption(UpdateOptionDto update)
        {
            var option =  await _repositoryManager.Option.GetOptionId(update.Id, true);
            if(option != null)
            {
                if (update.OptionName == null)
                {
                    return new BussnessResultModel(option, _locService.GetLocalizedStringValue("enterallfiled"), false);
                }
                _mapper.Map(update, option);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(option, _locService.GetLocalizedStringValue("successAdd"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"),false);
            }
        }
        public async Task<BussnessResultModel> DeleteOptionProduct(int id)
        {
            var option = await _repositoryManager.Option.GetOptionId(id, false);
            if (option != null)
            {
                var values = await _repositoryManager.Value.GetValuesOPtionId(id);
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        _repositoryManager.Value.DeleteValue(value);
                    }
                }
                var attributs = await _repositoryManager.Attribute.GetAttributesOptionId(id);
                if (attributs != null)
                {
                    foreach (var attribut in attributs)
                    {
                        _repositoryManager.Attribute.DeleteAttributesProduct(attribut);
                    }
                }
                _repositoryManager.Option.DeleteOption(option);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(option, _locService.GetLocalizedStringValue("successDelete"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"),false);
            }
        }
        public async Task<decimal> GetOptionsOrdersTotalPrice(int productId, int orderId)
        {
            decimal total = 0;
            var orderProduct = await _repositoryManager.OrderProducts.GetOrderProductsId(productId, orderId , false);
            if (orderProduct != null)
            {
                var special = await _repositoryManager.SpecialProducts.GetSpecialProductId(productId);
                var sale = await _repositoryManager.Sales.GetFlashProductId(productId);
                if (special != null && special.Id != 0)
                {
                    total = special.SpecialPrice;
                }
                else if (sale != null && sale.Id != 0)
                {
                    total = sale.DiscountPrice;
                }
                else
                {
                    total = orderProduct.FinalPrice;
                }
                var orderAttributProducts = await _repositoryManager.OrderAttributesProducts.GetAttributesOrderProduct(orderId, productId);
                if (orderAttributProducts.Count() > 0)
                {
                    foreach (var item in orderAttributProducts)
                    {
                        var attr = item.ProductAttribut;
                        var attributProduct = await _repositoryManager.Attribute.GetAttributeId(item.ProductAttributId.Value , true);

                        if (attributProduct.PricePrefix == "+")
                        {
                            total += Convert.ToDecimal(attributProduct.AttributePrice);
                        }

                        if (attributProduct.PricePrefix == "-")
                        {
                            if (total != 0)
                            {
                                total -= Convert.ToDecimal(attributProduct.AttributePrice);
                            }
                        }
                    }
                }
            }
            return total;
        }
        public async Task<List<OptionDto>> GetOptions(int productId)
        {
            var attributs = await _repositoryManager.Attribute.GetAttributesProductId(productId);
            var attrs = attributs.GroupBy(x => x.OptionId).Select(x => x.First()).ToList();
            var optionDto = new List<OptionDto>();

            if (attrs.Count() > 0)
            {
                foreach (var t in attrs)
                {
                    var valuesVM = new List<ValueVM>();
                    var option = await _repositoryManager.Option.GetOptionId(t.OptionId, false);
                    var values = attributs.Where(r => r.OptionId == option.Id).ToList();
                    if (values.Count() > 0)
                    {
                        foreach (var v in values)
                        {
                            var val = await _repositoryManager.Value.GetValueId(t.ValueId, false);
                            valuesVM.Add(new ValueVM
                            {
                                OptionId = val.OptionId,
                                OptionValueName = option.OptionName,

                                ValueId = val.Id,
                                OptionName = val.OptionValueName,
                                ValueHexModel = val.ValueHexModel,
                                IsDefault = v.IsDefault,

                                AttributeId = v.Id,
                                AttributePrice = (v.PricePrefix == "+" ? v.AttributePrice : -v.AttributePrice)
                            });
                        }
                    }
                    optionDto.Add(new OptionDto
                    {
                        Id = option.Id,
                        OptionName = option.OptionName,
                        OptionType = option.OptionType,
                        Values = valuesVM,
                    });
                }
            }
            return optionDto;
        }
        public async Task<PagedList<OptionDto>> GetProductOptions(PostsParameters postsParameters)
        {
            var options = await _repositoryManager.Option.GetAllOptions();
            var optionsDto = _mapper.Map<List<OptionDto>>(options);
            return PagedList<OptionDto>.ToPagedList(optionsDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        //Value------------------------------------------------
        public async Task<PagedList<ValueDto>> GetListValues(int optionId, PostsParameters postsParameters)
        {
            var value = await _repositoryManager.Value.GetValuesOPtionId(optionId);
            if (value == null) { return null; } 
            var valueDto = _mapper.Map<List<ValueDto>>(value);
            return PagedList<ValueDto>.ToPagedList(valueDto, postsParameters.PageNumber, postsParameters.PageSize);
            
        }
        public async Task<BussnessResultModel> AddValue(int optionId ,CreateValueDto createValueDto , string ValueHexModel = "#000000")
        {
            var option = await _repositoryManager.Option.GetOptionId(optionId, false);
            if (option == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("Error"),false);
            }
            var value = _mapper.Map<ProductOptionValue>(createValueDto);
            value.OptionId = optionId;
            value.ValueHexModel = (option.OptionType == "radio" ? "" : ValueHexModel);
            _repositoryManager.Value.CreateValue(value);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(value, _locService.GetLocalizedStringValue("successAdd"));
        }
        public async Task<BussnessResultModel> DeleteValueProduct(int valueId)
        {
            var value = await _repositoryManager.Value.GetValueId(valueId, false);
            if(value == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            _repositoryManager.Value.DeleteValue(value);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(value, _locService.GetLocalizedStringValue("successDelete"));
        }
        public async Task<BussnessResultModel> UpdateValue(UpdateValueDto updateDto)
        {
            var value = await _repositoryManager.Value.GetValueId(updateDto.Id, true);
            if (value == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            _mapper.Map(updateDto, value);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(value, _locService.GetLocalizedStringValue("successSave"));
        }
        //Review------------------------------------------------
        public async Task<BussnessResultModel> AddReview(int productId,int customerId, CreateReviewDto createReviewDto)
        {
            if(customerId > 0)
            {
                var reviewCustomer = await _repositoryManager.Review.GetReviewProductIdToCustomerId(productId, customerId, true);
                if (reviewCustomer == null)
                {
                    var review = _mapper.Map<Review>(createReviewDto);
                    review.ProductId = productId;
                    review.CustomerId = customerId;
                    review.IsStatus = Status.NotActive;
                    _repositoryManager.Review.AddReview(review);
                    await _repositoryManager.SaveAsync();
                    return new BussnessResultModel(review, "Your evaluation has been added under review");
                }
                else
                {
                    _mapper.Map(createReviewDto, reviewCustomer);
                    await _repositoryManager.SaveAsync();
                    return new BussnessResultModel(reviewCustomer, _locService.GetLocalizedStringValue("YourReviewUpdatedSuccessfully"));
                }
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("goLogin"),false);
            }
            
        }
        public async Task<BussnessResultModel> ActiveReview(int id)
        {
            var review = await _repositoryManager.Review.GetReviewId(id, true);
            if(review == null)
            {
                return new BussnessResultModel(null,_locService.GetLocalizedStringValue("correctLink"), false);
            }
            review.IsStatus = Status.Active;
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(review, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<BussnessResultModel> DeactiveReview(int id)
        {
            var review = await _repositoryManager.Review.GetReviewId(id, true);
            if (review == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            review.IsStatus = Status.NotActive;
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(review, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<bool> IsReview(int productId, int customerId)
        {
            var IsReview = false;
            if (customerId == 0)
            {
                return false;
            }
            var review = await _repositoryManager.Review.GetActiveReviewProductCustomer(productId , customerId , true);
            if (review != null)
            {
                IsReview = true;
            }
            return IsReview;
        }
        public async Task<List<ReviewDto>> GetLast3Reviews(int productId)
        {
            var reviews = await _repositoryManager.Review.Last3Reviews(productId);
            var reviewsDto = reviews.Select(review=>new ReviewDto
            {
                Id = review.Id,
                Rating = review.Rating,
                Text = review.Text,
                CustomerId = review.CustomerId,
                CustomerName = review.Customer.FullName,
                CustomerImage = review.Customer.Avater ?? null,
                ProductId = productId
            }).ToList();
            return reviewsDto;
        }
        public async Task<List<ReviewDto>> GetActiveReviews(int productId)
        {
            var reviews = await _repositoryManager.Review.GetReviewsActiveProductId(productId);
            var reviewsDto = reviews.Select(review => new ReviewDto
            {
                Id = review.Id,
                Rating = review.Rating,
                Text = review.Text,
                CustomerId = review.CustomerId,
                CustomerName = review.Customer.FullName,
                CustomerImage = review.Customer.Avater ?? null,
                ProductName = review.Product.ProductName,
                ProductId = productId
            }).ToList();
            return reviewsDto;
        } 
        public async Task<PagedList<ReviewDto>> GetAllReviews(string lang , PostsParameters postsParameters)
        {
            var reviews = await _repositoryManager.Review.GetReviews();
            var reviewsDto = new List<ReviewDto>();
            foreach(var review in reviews)
            {
                reviewsDto.Add(new ReviewDto
                {
                    Id = review.Id,
                    ProductName = lang == "en" ? review.Product.ProductName : review.Product.ProductNameAr,
                    Text = review.Text,
                    IsStatus = review.IsStatus,
                    CreatedAt = review.CreatedAt.ToString("MM/dd/yyyy hh:mm tt"),
                });
            }
            return PagedList<ReviewDto>.ToPagedList(reviewsDto, postsParameters.PageNumber, postsParameters.PageSize);
        } 
        public async Task<decimal> Rate(int productId)
        {
            var reviews = await _repositoryManager.Review.GetReviewsProductId(productId);
            decimal rate = (reviews.Count() > 0 ? Convert.ToDecimal(reviews.Sum(r => r.Rating) / reviews.Count()) : 0);
            return rate;
        }
        //WishList------------------------------------------------
        public async Task<BussnessResultModel> AddWishList(int customerId , int productId)
        {
            var like = await _repositoryManager.WishList.GetWishListProductIdCustomerId(customerId, productId);
            if (like == null)
            {
                var wishList = new WishList{ ProductId = productId, CustomerId = customerId };
                _repositoryManager.WishList.Addlike(productId, wishList);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(wishList, "successfully Added");
            }
            else
            {
                _repositoryManager.WishList.DeleteLike(like);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(like, _locService.GetLocalizedStringValue("successDelete"));
            }
        }
        public async Task<BussnessResultModel> DeleteLike(int id, int customerId)
        {
            var wishList = await _repositoryManager.WishList.GetLikeCustomerId(id, customerId);
            if (wishList == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            _repositoryManager.WishList.DeleteLike(wishList);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(wishList, _locService.GetLocalizedStringValue("successDelete"));
        }
        public async Task<bool> IsFavourite(int customerId, int productId)
        {
            bool favorit = false;
            var WishList = await _repositoryManager.WishList.GetWishListProductIdCustomerId(customerId, productId);
            if (WishList != null)
            {
                favorit = true;
            }
            return favorit;
        }
        public async Task<int> GetFavourite(int customerId, int productId)
        {
            int favId = 0;
            var WishList = await _repositoryManager.WishList.GetWishListProductIdCustomerId(customerId, productId);
            if (WishList != null)
            {
                favId = WishList.Id;
            }
            return favId;
        }
        //inventory------------------------------------------------
        public async Task<PagedList<InventoryDto>> GetAllInventory(int userId ,string lang , PostsParameters postsParameters)
        {
            var stocks = await _repositoryManager.Inventory.GetAllInventory();
            var store = await _repositoryManager.User.GetActiveUserId(userId, false);
            if(store.UserType == UserType.Store)
            {
                stocks.Where(c => c.VendorId == userId);
            }
            var chatGrouped = stocks.GroupBy(c => c.Product)
                .Select(x => new
                {
                    ProductName =  lang == "en" ? x.Key.ProductName : x.Key.ProductNameAr,
                    SumStock = AvailabilityProducts(x.Key.Id),
                }).Distinct();
            var i = 1;
            var stocksDto = chatGrouped.Select(stock => new InventoryDto
            {
                Id = i++,
                Stock =  stock.SumStock,
                ProductName = stock.ProductName,
            }).ToList();
           
            return PagedList<InventoryDto>.ToPagedList((IEnumerable<InventoryDto>)stocksDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<PagedList<InventoryDto>> GetAllOutInventory(int userId, string lang, PostsParameters postsParameters)
        {
            var stocks = await _repositoryManager.Inventory.GetAllInventory();
            var stocksDto = new List<InventoryDto>();
            var store = await _repositoryManager.User.GetActiveUserId(userId, false);
            if (store.UserType == UserType.Store)
            {
                stocks.Where(c => c.VendorId == userId);
            }
            foreach (var stock in stocks)
            {
                int inSum = 0;
                var inStocks = await _repositoryManager.Inventory.GetInStockProduct(stock.ProductId);
                if(inStocks != null)
                {
                    inSum = inStocks.Sum(c=>c.Stock);
                }
                int outSum = 0;
                var outStocks = await _repositoryManager.Inventory.GetOutStockProduct(stock.ProductId);
                if (outStocks != null)
                {
                    outSum = outStocks.Sum(c => c.Stock);
                }
                if ((inSum - outSum) == 0)
                {
                    stocksDto.Add(new InventoryDto
                    {
                        Id = stock.Id,
                        ProductId = stock.ProductId,
                        ProductName = lang == "en" ? stock.Product.ProductName : stock.Product.ProductNameAr,
                        UpdateAt = stock.UpdatedAt.Value.ToString("MM/dd/yyyy hh:mm tt") ?? null
                    });
                }
            }
            return PagedList<InventoryDto>.ToPagedList((IEnumerable<InventoryDto>)stocksDto.DistinctBy(c => c.ProductId), postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<List<InventoryDto>> GetAllViewInventory(int userId, string lang, int productId)
        {
            var stocks = await _repositoryManager.Inventory.GetAllInventoryByPrductId(productId);
            var store = await _repositoryManager.User.GetActiveUserId(userId, false);
            if (store.UserType == UserType.Store)
            {
                stocks.Where(c => c.VendorId == userId);
            }
            int inSum = 0;
            var inStocks = await _repositoryManager.Inventory.GetInStockProduct(productId);
            if (inStocks != null)
            {
                inSum = inStocks.Sum(c => c.Stock);
            }
            int outSum = 0;
            var outStocks = await _repositoryManager.Inventory.GetOutStockProduct(productId);
            if (outStocks != null)
            {
                outSum = outStocks.Sum(c => c.Stock);
            }

            var stocksDto = stocks.Select(stock => new InventoryDto
            {
                Id = stock.Id,
                AddedBy = store.FullName ,
                ProductId = productId,
                ProductName = lang == "en" ? stock.Product.ProductName : stock.Product.ProductNameAr,
                CreatedAt = stock.CreatedAt.ToString("MM/dd/yyyy hh:mm tt"),
                StockType  = stock.StockType,   
                Stock = stock.Stock,
                PurchaseCode = stock.PurchaseCode,
                Total = inSum - outSum
            }).ToList();
           return stocksDto;
        }
        public async Task<BussnessResultModel> AddInventory(int userId, CreateInventoryDto createDto)
        {
            var inventory = _mapper.Map<Inventory>(createDto);
            var item = await _repositoryManager.Inventory.GetOptionsByProductIdInStock(createDto.ProductId);
            if (item !=null)
            {
               await DeleteInventory(createDto.ProductId, createDto.AttributesProductId);
            }
            var product = await _repositoryManager.Product.GetProductById(createDto.ProductId, true);
            product.Availability = product.Availability + createDto.Stock;
            var user = await _repositoryManager.User.GetUserId(userId, false);
            if (user.UserType == UserType.Admin)
            {
                inventory.AdminId = userId;
            }
            else
            {
                inventory.VendorId = userId;
            }
            inventory.StockType = "in";
            inventory.AddedDate = _util.EasternTime.Millisecond;
            _repositoryManager.Inventory.AddInventory(inventory);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(inventory, _locService.GetLocalizedStringValue("successAdd"));
        }
        public async Task DeleteInventory(int productId, int? attr)
        {
            var attrp = await _repositoryManager.Inventory.GetOptionsByProductIdInStock(productId);
            if (attr != null)
            {
                attrp = await _repositoryManager.Inventory.GetProductIdOptoinIdInStock(productId, attr.Value);
            }
            foreach (var t in attrp)
            {
                _repositoryManager.Inventory.DeleteInventory(t);
            }
            await _repositoryManager.SaveAsync();
        }
        public int AvailabilityProductOption(int productId , int attributeId)
        {
            var total = 0;
            var instock = 0;
            var outstock = 0;

            var inventories = _repositoryManager.Inventory.GetAllInventoryByProductIdOption(productId, attributeId).Result;
            if (inventories != null)
            {
                foreach (var inventory in inventories)
                {
                    if (inventory.StockType == "in")
                    {
                        instock += inventory.Stock;
                    }
                    if (inventory.StockType == "out")
                    {
                        outstock += inventory.Stock;
                    }
                }
            }
            if ((instock - outstock) > 0)
            {
                return total = instock - outstock;
            }
            else
            {
                return total;
            }
        }
        public int AvailabilityProducts(int productId)
        {
            var total = 0;
            var instock = 0;
            var outstock = 0;

            var inventories = _repositoryManager.Inventory.GetAllInventoryByPrductId(productId).Result;
            if (inventories != null)
            {
                foreach (var inventory in inventories)
                {
                    if (inventory.StockType == "in")
                    {
                        instock += inventory.Stock;
                    }
                    if (inventory.StockType == "out")
                    {
                        outstock += inventory.Stock;
                    }
                }
            }
            if ((instock - outstock) > 0)
            {
                return total = instock - outstock;
            }
            else
            {
                return total;
            }
        }
    }
}
