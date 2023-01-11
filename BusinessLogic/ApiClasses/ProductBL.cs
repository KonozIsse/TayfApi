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
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BusinessLogic.ApiClasses
{
    public class ProductBL
    {
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper;
        private readonly ImageBL _imageBL;
        private readonly Util _util;
        private readonly LocService _locService;
        private readonly IConfiguration _configuration;
        public ProductBL(IRepositoryManager repositoryManager, IMapper mapper, ImageBL imageBL, Util util , LocService locService, IConfiguration configuration)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _imageBL = imageBL;
            _util = util;
            _locService = locService;
            _configuration = configuration;
        }
        //Category------------------------------------------------
        public async Task<List<CategoryDto>> GetAllCategories(string lang = "en")
        {
            var categories = await _repositoryManager.Categories.GetAllCategories(false);
            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories);
            var category = categories.First();
            var categoryDto = categoriesDto.First();
            categoryDto.CategoryName = lang == "en" ? category.CategoryName : category.CategoryNameAr;
            return categoriesDto;
        }
        public async Task<CategoryDto> GetCategory(int catId, string lang = "en")
        {
            var category = await _repositoryManager.Categories.GetCategoryById(catId ,false);
            var categoryDto = _mapper.Map<CategoryDto>(category);
            categoryDto.CategoryName = lang == "en" ? category.CategoryName : category.CategoryNameAr;
            return categoryDto;
        }
        public async Task<List<CategoryDto>> GetSearchMainCategories(string search, string lang = "en")
        {
            var categories = await _repositoryManager.Categories.SearchMainCategoriesCP(search);
            var mainCategoryDto = _mapper.Map<List<CategoryDto>>(categories);
            var category = categories.First();
            var categoryDto = mainCategoryDto.First();
            categoryDto.CategoryName = lang == "en" ? category.CategoryName : category.CategoryNameAr;
            return mainCategoryDto;
        }
        public async Task<IEnumerable<CategoryDto>> SubCategoriesMainId(string lang ="en")
        {
           var categories= await _repositoryManager.Categories.SubCategoriesMainId();
            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories);
            var category = categories.First();
            var categoryDto = categoriesDto.First();
            categoryDto.CategoryName = lang == "en" ? category.CategoryName : category.CategoryNameAr;
            return categoriesDto;
        }
        public async Task<List<CategoryDto>> GetSubActiveCategories(string lang = "en")
        {
            var categories = await _repositoryManager.Categories.GetSubActiveCategories(false);
            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories);
            var category = categories.First();
            foreach (var categoryDto in categories)
            {
                categoryDto.CategoryName = lang == "en" ? category.CategoryName : category.CategoryNameAr;
                //categoryDto.Products.Where(c => c.IsStatus == Status.Active);
            }
            return categoriesDto;
        }
        public async Task AddCategoriesProduct(int productId ,List<CategoriesProductDto> list)
        {
            var product = await _repositoryManager.Product.GetProductById(productId, false);
            if(product != null) 
            {
                var categoriesProdId = await _repositoryManager.ProductCategory.GetAllCategoriesProdId(productId, false);
                if (categoriesProdId != null)
                {
                    foreach (var category in categoriesProdId)
                    {
                        _repositoryManager.ProductCategory.DeleteProductCategory(category);
                        await _repositoryManager.SaveAsync();
                    }
                }
                //product.ProductCategories.AddRange(_mapper.Map<List<ProductCategory>>(list));

                foreach (var item in list)
                {
                    var cat = _mapper.Map<ProductCategory>(item);
                    cat.ProductId = productId;
                    _repositoryManager.ProductCategory.CreateProductCategory(cat);
                }

            } 
            await _repositoryManager.SaveAsync();
        }
       
        public async Task<List<CategoryDto>> GetMainCategories(string lang = "en")
        {
            var categories = await _repositoryManager.Categories.GetSubCategories(false);
            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories);
            var category = categories.First();
            var categoryDto = categoriesDto.First();
            categoryDto.CategoryName = lang == "en" ? category.CategoryName : category.CategoryNameAr;
            return categoriesDto;
        }
        public async Task<List<CategoryDto>> GetSubCategoriesCP(int mainId, string lang = "en")
        {
            var categories = await _repositoryManager.Categories.GetSubCategoriesMainIDCP(mainId);
            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories);
            var category = categories.First();
            var categoryDto = categoriesDto.First();
            categoryDto.CategoryName = lang == "en" ? category.CategoryName : category.CategoryNameAr;
            return categoriesDto;
        } 
        public async Task<List<CategoryDto>> GetSubCategoriesForHome(int mainId, string lang = "en")
        {
            var categories = await _repositoryManager.Categories.GetSubActiveCategories(false);
            if(mainId != 0)
            {
                categories = await _repositoryManager.Categories.GetSubCategoriesByMainId(mainId, false);
            }
            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories);
            //var category = categories.First();
            //var categoryDto = categoriesDto.First();
            //categoryDto.CategoryName = lang == "en" ? category.CategoryName : category.CategoryNameAr;
            return categoriesDto;
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
            if (create.MainCategoryId == null)
            {
                category.MainCategoryId = 0;
            }
            else
            {
                category.MainCategoryId = create.MainCategoryId.Value;
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
        public async Task<List<StoreDto>> GetStoresByProductList(List<ProductVM> productsModel)
        {
            List<StoreDto> model = new List<StoreDto>();
            var stores = productsModel.GroupBy(x => x.StoreId).Select(x => x.First()).ToList();
            foreach (var store in stores)
            {
                if (store.StoreId != null && store.StoreId != 0)
                {
                    var storeDB = await _repositoryManager.User.GetStoreId(Convert.ToInt32(store.StoreId));
                    if (storeDB != null)
                    {
                        model.Add(new StoreDto
                        {
                            Id = storeDB.Id,
                            FirstName = storeDB.FirstName,
                            Avater = storeDB.Avater,
                            AdressInfo = storeDB.AdressInfo
                        });
                    }
                }
            }
            return model;
        }
        public async Task<List<Product>> GetProductsByVendor(int storeId)
        {
            return await _repositoryManager.Product.GetProductsTOStoreId(storeId);
        } 
        public async Task<List<Product>> GetAllProducts()
        {
            return await _repositoryManager.Product.GetProducts();
        }  
        public async Task<Product> GetProductId(int id)
        {
            return await _repositoryManager.Product.GetProductById(id , false);
        }
        public async Task<BussnessResultModel> AddProduct(CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);
            if (createProductDto.DescriptionAr == null || createProductDto.ProductNameAr == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterallfiled"), false);
            }
            if (createProductDto.StoreId != null)
            {
                product.StoreId = createProductDto.StoreId;
                product.IsAcceptAdmin = false;
            }
            else
            {
                product.IsAcceptAdmin = true;
            }
            _repositoryManager.Product.AddProduct(product);
            await _repositoryManager.SaveAsync();
            if (createProductDto.ProductCategories != null)
            {
                //await AddCategoriesProduct(product.Id, createProductDto.ProductCategories);
                product.ProductCategories.AddRange(_mapper.Map<List<ProductCategory>>(createProductDto.ProductCategories));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("selectcategory"), false);
            }
            if (createProductDto.imagesProduct != null)
            {
                foreach(var nameImage in createProductDto.imagesProduct)
                {
                    var image = new Image
                    {
                        Name = nameImage,
                        ProductId = product.Id,
                        VendId = product.StoreId == 0 ? 0 : product.StoreId
                    };
                    _repositoryManager.Image.AddImage(image);
                }
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctImage"), false);
            }
            if (createProductDto.Price < 0)
            {
                return new BussnessResultModel(product, _locService.GetLocalizedStringValue("enterPrice"), false);
            }
            if (product.IsSale == true)
            {
                var craete = createProductDto.ProductSales.First();
               
                var sale = _mapper.Map<ProductSales>(craete);
                if (sale.StartDate > sale.EndDate)
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("StartDateMustBeGaraterThanExpireDate"), false);
                }
                _repositoryManager.Sales.AddFlashSale(sale);
            }
            if (product.IsSpecial == true)
            {
                var special = _mapper.Map<SpecialProducts>(createProductDto.SpecialProducts.First());
                if (_util.EasternTime > createProductDto.SpecialProducts.First().EndDate)
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("StartDateMustBeGaraterThanExpireDate"), false);
                }
                _repositoryManager.SpecialProducts.AddSpecialProduct(special);
            }
            if (product.Availability != 0)
            {
                var createInventory = new CreateInventoryDto();
                var inventory = _mapper.Map<Inventory>(createInventory);
                inventory.Stock = product.Availability;
                inventory.StockType = "in";
                inventory.AddedDate = _util.EasternTime.Millisecond;
                inventory.VendorId = product.StoreId == 0 ? 0 : product.StoreId;
               _repositoryManager.Inventory.AddInventory(inventory);
            }
            return new BussnessResultModel(product , _locService.GetLocalizedStringValue("successAdd"));
        }
        public async Task<BussnessResultModel> EditProduct(UpdateProductDto updateDto)
        {
            var product = await _repositoryManager.Product.GetProductById(updateDto.Id, true);
            if(product == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            product.StoreId = updateDto.StoreId;
            if (updateDto.DescriptionAr == null || updateDto.ProductNameAr == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterallfiled"), false);
            }
            if (updateDto.Price < 0)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterPrice"), false);
            }
            _mapper.Map(updateDto, product);
            if (updateDto.ProductCategories != null)
            {
                await AddCategoriesProduct(updateDto.Id, updateDto.ProductCategories);
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("selectcategory"), false);
            }
            if (updateDto.imagesProduct != null)
            {
                foreach (var nameImage in updateDto.imagesProduct)
                {
                    var image = new Image
                    {
                        Name = nameImage,
                        ProductId = product.Id,
                        VendId = product.StoreId == 0 ? 0 : product.StoreId
                    };
                    _repositoryManager.Image.AddImage(image);
                }
            }
            else
            {
                 return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctImage"), false);
            }
            if (product.IsSale == true )
            {
                var sale = updateDto.ProductSales.First();
                if (sale.StartDate > sale.EndDate)
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("StartDateMustBeGaraterThanExpireDate"),false);
                }
                var salesProduct = await _repositoryManager.Sales.CheckFlashExists(updateDto.Id, true);
                if (salesProduct != null)
                {
                    salesProduct.ProductId = updateDto.Id;
                    _mapper.Map(updateDto.ProductSales.First(), salesProduct);
                }
                else
                {
                    var sales = _mapper.Map<ProductSales>(updateDto.ProductSales.First());
                    _repositoryManager.Sales.AddFlashSale(sales);
                }
               
            }
            else 
            {
                var salesProduct = await _repositoryManager.Sales.CheckFlashExists(updateDto.Id, false);
                if (salesProduct != null)
                {
                    _repositoryManager.Sales.DeleteFlashSale(salesProduct);
                }
            }
            if (product.IsSpecial == true )
            {
                if (_util.EasternTime > updateDto.SpecialProducts.First().EndDate)
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("StartDateMustBeGaraterThanExpireDate"), false); 
                }
                var exait = await _repositoryManager.SpecialProducts.CheckSpecialExists(updateDto.Id, true);
                if(exait != null)
                {
                    exait.ProductId = updateDto.Id;
                    _mapper.Map(updateDto.SpecialProducts.First(), exait);
                }
                else
                {
                    var special = _mapper.Map<SpecialProducts>(updateDto.SpecialProducts.First());
                    _repositoryManager.SpecialProducts.AddSpecialProduct(special);
                }
                
            }
            else
            {
                var special = await _repositoryManager.SpecialProducts.CheckSpecialExists(updateDto.Id,false);
                if (special != null)
                {
                    _repositoryManager.SpecialProducts.DeleteSpecialProduct(special);
                }
            }
          
            await _repositoryManager.SaveAsync();
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
        public async Task<Product> CheckApproveProduct(int productId)
        {
          return await _repositoryManager.Product.CheckApproveProduct(productId);
        } 
        public async Task<List<OrderProduct>> GetOrdersProduct(int productId)
        {
          return await _repositoryManager.OrderProducts.GetOrdersProductId(productId);
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
                var specials = await _repositoryManager.SpecialProducts.GetSpecialProductsProductId(productId);
                if (specials != null)
                {
                    foreach (var special in specials)
                    {
                        _repositoryManager.SpecialProducts.DeleteSpecialProduct(special);
                    }
                }
                var sales = await _repositoryManager.Sales.GetAllSalesProductId(productId);
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
        public async Task<List<ProductVM>> GetProducts(int customerId = 0, string lang = "en")
        {
            var products = await _repositoryManager.Product.GetAllAcceptedProducts();
            var productsDto = new List<ProductVM>();
            
            if (products != null)
            {
                foreach (var product in products)
                {
                    var category = await _repositoryManager.ProductCategory.GetCategoryToPrductId(product.Id);
                    var special = await IsOffer(product.Id);
                    var flash = await _repositoryManager.Sales.GetFlashProductId(product.Id);
                    if (product != null)
                    {
                        productsDto.Add(new ProductVM
                        {
                            MainCategoryId = category.Category.MainCategoryId ,
                            CategoryId = category.Id ,
                            CategoryName = lang == "en" ? category.Category.CategoryName : category.Category.CategoryNameAr,
                           // CategoryImage = (category != null ? await _imageBL.GetImageThumbnail(category.ImgId.ToString()) : ""),

                            Id = product.Id,
                            ProductName = lang == "en" ? product.ProductName : product.ProductNameAr,
                            Description = lang == "en" ? product.Description : product.DescriptionAr,
                            ProductModel = product.ProductModel,
                            //ProductImage = await _imageBL.GetImageThumbnail(product.Images.First().Id.ToString()),
                            TypeId = product.TypeId,
                            Price = product.Price,
                            IsStatus = product.IsStatus.ToString(),
                            Availability = await AvailabilityProducts(product.Id),
                            Attributs = await GetOptions(product.Id),
                            Images = await _imageBL.GetListImagesProductIdAsync(product.Id),
                            ShareLink = _util.url1 + "/share.html?id=" + product.Id,
                            IsBest = Convert.ToInt16(product.IsBest),
                            IsFeature = product.IsFeature,

                            IsSpecial = product.IsSpecial,
                            SpecialPrice = product.IsSpecial == false ? 0 : special.SpecialPrice,

                            IsSale = product.IsSale,
                            DiscountPrice = (flash != null ? flash.DiscountPrice : 0), 
                            StartDate = (flash != null ? flash.StartDate : null),
                            EndDate = (flash != null ? flash.EndDate : null),


                            IsFavorite =   await IsFavourite(customerId, product.Id),
                            NumLike = await GetFavourite(customerId, product.Id),
                            IsReview = await IsReview(customerId, product.Id),
                            Reviews = await GetReviews(product.Id),
                            Rate = await Rate(product.Id),

                            StoreId = product.StoreId == null ? null : product.StoreId,
                            StoreName = product.Store != null ?  product.Store.FullName : null,
                            StoreImage = product.Store == null ? null : product.Store.Avater
                            
                        }) ; 
                    }
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
                return new List<ProductVM>();
            }
        }
        public async Task<List<ProductVM>> GetProductsHome(int customerId,string search, string lang = "en")
        {
            var products = await _repositoryManager.Product.GetAllAcceptedProducts();
            var productsDto = new List<ProductVM>();

            if (products != null)
            {
                foreach (var product in products)
                {
                    var category = await _repositoryManager.ProductCategory.GetCategoryToPrductId(product.Id);
                    var special = await IsOffer(product.Id);
                    var flash = await _repositoryManager.Sales.GetFlashProductId(product.Id);
                    if (product != null)
                    {
                        productsDto.Add(new ProductVM
                        {
                            MainCategoryId = category.Category.MainCategoryId,
                            CategoryId = category.Id,
                            CategoryName = lang == "en" ? category.Category.CategoryName : category.Category.CategoryNameAr,
                            // CategoryImage = (category != null ? await _imageBL.GetImageThumbnail(category.ImgId.ToString()) : ""),

                            Id = product.Id,
                            ProductName = lang == "en" ? product.ProductName : product.ProductNameAr,
                            Description = lang == "en" ? product.Description : product.DescriptionAr,
                            ProductModel = product.ProductModel,
                            //ProductImage = await _imageBL.GetImageThumbnail(product.Images.First().Id.ToString()),
                            TypeId = product.TypeId,
                            Price = product.Price,
                            IsStatus = product.IsStatus.ToString(),
                            Availability = await AvailabilityProducts(product.Id),
                            Attributs = await GetOptions(product.Id),
                            Images = await _imageBL.GetListImagesProductIdAsync(product.Id),
                            ShareLink = _util.url1 + "/share.html?id=" + product.Id,
                            IsBest = Convert.ToInt16(product.IsBest),
                            IsFeature = product.IsFeature,

                            IsSpecial = product.IsSpecial,
                            SpecialPrice = product.IsSpecial == false ? 0 : special.SpecialPrice,

                            IsSale = product.IsSale,
                            DiscountPrice = (flash != null ? flash.DiscountPrice : 0),
                            StartDate = (flash != null ? flash.StartDate : null),
                            EndDate = (flash != null ? flash.EndDate : null),


                            IsFavorite = await IsFavourite(customerId, product.Id),
                            NumLike = await GetFavourite(customerId, product.Id),
                            IsReview = await IsReview(customerId, product.Id),
                            Reviews = await GetReviews(product.Id),
                            Rate = await Rate(product.Id),

                            StoreId = product.StoreId == null ? null : product.StoreId,
                            StoreName = product.Store != null ? product.Store.FullName : null,
                            StoreImage = product.Store == null ? null : product.Store.Avater

                        });
                    }
                }
                foreach (var item in productsDto)
                {
                    if (item.IsSale == true && products.First().ProductSales != null)
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
                return new List<ProductVM>();
            }
        }
        public async Task<PagedList<ProductDto>> GetProductsCP(int? storeId , string search , string lang, PostsParameters postsParameters)
        {
            var products = await _repositoryManager.Product.GetProductsCP(storeId, search, postsParameters);
            //var productsDto =_mapper.Map<IEnumerable<ProductDto>>(products);
            var productsDto = new List<ProductDto>();
            foreach (var item in products)
            {
                var cat = await _repositoryManager.ProductCategory.GetCategoryToPrductId(item.Id);
                var catName = lang == "en" ? cat.Category.CategoryName : cat.Category.CategoryNameAr;
                productsDto.Add(new ProductDto
                {
                    Id = item.Id,
                    ProductName = lang == "en" ? item.ProductName : item.ProductNameAr,
                    Description = lang == "en" ? item.Description : item.DescriptionAr,
                    IsStatus = item.IsStatus,
                    ImageProduct = item.ImageProduct,
                    CategoryName = catName,
                    IsAcceptAdmin = item.IsAcceptAdmin
                });
            }
            return (PagedList<ProductDto>)productsDto;
        }
        public async Task<ProductVM> GetDetailProduct(int productId, int? customerId , string lang )
        {
            var special = await IsOffer(productId);
            var flash = await _repositoryManager.Sales.GetFlashProductId(productId);
            var category = await _repositoryManager.ProductCategory.GetCategoryToPrductId(productId);
            var product = await _repositoryManager.Product.GetAcceptAdminActiveProduct(productId);
            if(product == null) { return null; }
            return new ProductVM
            {
                MainCategoryId = category.Category.MainCategoryId,
                CategoryId = category.Id,
                CategoryName = lang == "en" ? category.Category.CategoryName : category.Category.CategoryNameAr,
                // CategoryImage = (category != null ? await _imageBL.GetImageThumbnail(category.ImgId.ToString()) : ""),

                Id = product.Id,
                ProductName = lang == "en" ? product.ProductName : product.ProductNameAr,
                Description = lang == "en" ? product.Description : product.DescriptionAr,
                ProductModel = product.ProductModel,
                ProductImage = await _imageBL.GetImageThumbnail(product.Images.First().Id.ToString()),
                TypeId = product.TypeId,
                Price = product.Price,
                IsStatus = product.IsStatus.ToString(),
                Availability = await AvailabilityProducts(product.Id),
                Attributs = await GetOptions(product.Id),
                Images = await _imageBL.GetListImagesProductIdAsync(product.Id),
                //ShareLink = storeUrl + "/en/Home/share?id=" + product.Id + "&name=" + name.Trim(),
                IsBest = Convert.ToInt16(product.IsBest),
                IsFeature = product.IsFeature,

                IsSpecial = product.IsSpecial,
                SpecialPrice = product.IsSpecial == false ? 0 : special.SpecialPrice,

                IsSale = product.IsSale,
                DiscountPrice = (flash != null ? flash.DiscountPrice : 0),
                StartDate = (flash != null ? flash.StartDate : null),
                EndDate = (flash != null ? flash.EndDate : null),


                IsFavorite = await IsFavourite(customerId.Value, product.Id),
                NumLike = await GetFavourite(customerId.Value, product.Id),
                IsReview = await IsReview(customerId.Value, product.Id),
                Reviews = await GetReviews(product.Id),
                Rate = await Rate(product.Id),

                StoreId = product.StoreId,
                StoreName = product.StoreId == null ? null : product.Store.FullName,
                StoreImage = product.StoreId == null ? null : product.Store.Avater
            };
        }
        public async Task<ProductVM> GetProductDetails(int productId, int customerId, string lang = "en", string storeUrl = null)
        {
            var special = await IsOffer(productId);
            var flash = await _repositoryManager.Sales.GetFlashProductId(productId);
            var category = await _repositoryManager.ProductCategory.GetCategoryToPrductId(productId);
            var product = await _repositoryManager.Product.GetProductById(productId, false);
            if (product == null) { return null; }
            var name = lang == "en" ? product.ProductName : product.ProductNameAr;
            var descreption = lang == "en" ? product.Description : product.DescriptionAr;
            var filesRootPath = _configuration.GetSection("filesRootPath").Value;

            if (string.IsNullOrEmpty(storeUrl))
            {
                storeUrl = _configuration.GetSection("storeUrl").Value;
            }
            return new ProductVM
            {
                MainCategoryId = category.Category.MainCategoryId,
                CategoryId = category.Id,
                CategoryName = lang == "en" ? category.Category.CategoryName : category.Category.CategoryNameAr,
                // CategoryImage = (category != null ? await _imageBL.GetImageThumbnail(category.ImgId.ToString()) : ""),

                Id = product.Id,
                ProductName = name,
                Description = descreption,
                ProductModel = product.ProductModel,
                ProductImage = await _imageBL.GetImageThumbnail(product.Images.First().Id.ToString()),
                TypeId = product.TypeId,
                Price = product.Price,
                IsStatus = product.IsStatus.ToString(),
                Availability = await AvailabilityProducts(product.Id),
                Attributs = await GetOptions(product.Id),
                Images = await _imageBL.GetListImagesProductIdAsync(product.Id),
                ShareLink = storeUrl + "/en/Home/share?id=" + product.Id + "&name=" + name.Trim(),
                IsBest = Convert.ToInt16(product.IsBest),
                IsFeature = product.IsFeature,

                IsSpecial = product.IsSpecial,
                SpecialPrice = product.IsSpecial == false ? 0 : special.SpecialPrice,

                IsSale = product.IsSale,
                DiscountPrice = (flash != null ? flash.DiscountPrice : 0),
                StartDate = (flash != null ? flash.StartDate : null),
                EndDate = (flash != null ? flash.EndDate : null),


                IsFavorite = await IsFavourite(customerId, product.Id) ,
                NumLike = await GetFavourite(customerId, product.Id) ,
                IsReview = await IsReview(customerId, product.Id),
                Reviews = await GetReviews(product.Id)??null,
                Rate = await Rate(product.Id),

                StoreId = product.StoreId,
                StoreName = product.StoreId == null ? null : product.Store.FullName,
                StoreImage = product.StoreId == null ? null : product.Store.Avater
            };
        }
        public async Task<List<ProductVM>> GetProductsCatId(int catId, int customerId, string lang  , Currency currency = null)
        {
            var products = await GetProducts(customerId, lang);
            var productsCatId = products.Where(c => c.CategoryId == catId).ToList();

            return productsCatId;
        }
        public async Task<List<ProductVM>> GetProductsStore(int storeId, int customerId, string lang, Currency currency)
        {
            var products = await GetProducts(customerId, lang);
            var productsCatId = products.Where(c => c.StoreId == storeId).ToList();

            return productsCatId;
        }
        public async Task<List<ProductDto>> GetProductsDto()
        {
            var products = await _repositoryManager.Product.GetProducts();
            var productsDto = _mapper.Map<List<ProductDto>>(products) ;
            return productsDto;
        }
        public async Task<List<ProductVM>> GetSpecialsProd(int customerId = 0 , string lang = "",Currency currency =null )
        {
            var products = await GetProducts(customerId, lang);
            var productsCatId = products.Where(c => c.IsSpecial == true).ToList();
            return productsCatId;
        }
        public async Task<List<ProductVM>> GetFlashProds()
        {
            var products = await GetProducts();
            var productsCatId = products.Where(c => c.IsSale == true).ToList();
            return productsCatId;
        }
        public async Task<List<ProductVM>> GetWishProduct(int customerId ,string lang , Currency currency)
        {
            var products = await GetProducts(customerId, lang);
            var likes = await _repositoryManager.WishList.GetLikesCustomerId(customerId);
            var productsCatId = products.Where(c => likes.Any(x=> c.Id == x.ProductId)).ToList();
            return productsCatId;
        }
        public async Task<SpecialDto> IsOffer(int productId)
        {
            var special = await _repositoryManager.SpecialProducts.GetSpecialProductId(productId);
            if (special == null)
            {
                return null;
            }
            var specialDto = _mapper.Map<SpecialDto>(special);
            special.ProductId = productId;
            return specialDto;
        }
        public async Task<List<ProductPageDto>> PopularsPage(int pageSize = 10)
        {
            var popular = await _repositoryManager.Product.GetPopularProducts(pageSize);
            var popularDto = _mapper.Map<List<ProductPageDto>>(popular);
            return popularDto;
        }
        public async Task<List<ProductPageDto>> BestPage(int pageSize = 10)
        {
            var popular = await _repositoryManager.Product.GetBestProducts(pageSize);
            var popularDto = _mapper.Map<List<ProductPageDto>>(popular);
            return popularDto;
        }
        public async Task<List<ProductPageDto>> LatestPage(int pageSize = 10)
        {
            var populars = await _repositoryManager.Product.GetLatestPage(pageSize);
            var popularsDto = _mapper.Map<List<ProductPageDto>>(populars);
            return popularsDto;
        }
        public async Task<List<ProductPageDto>> SpecialsPage(int pageSize = 5)
        {
            var populars = await _repositoryManager.Product.SpecialsPage(pageSize);
            var popularsDto = _mapper.Map<List<ProductPageDto>>(populars);
            return popularsDto;
        }
        public async Task<List<ProductPageDto>> TopRatedPage(int pageSize = 6)
        {
            var populars = await _repositoryManager.Product.TopRatedPage(pageSize);
            var popularsDto = _mapper.Map<List<ProductPageDto>>(populars);
            return popularsDto;
        }
        public async Task<List<ProductPageDto>> DailyDeals()
        {
            var populars = await _repositoryManager.Product.DailyDeals();
            var popularsDto = _mapper.Map<List<ProductPageDto>>(populars);
            return popularsDto;
        }
        public async Task<List<ProductDto>> GetProductsCP(int storeId , string seacrh , int catId)
        {
            var populars = await _repositoryManager.Product.SearshProductByCategoryAndStore(storeId, seacrh,catId);
            var popularsDto = _mapper.Map<List<ProductDto>>(populars);
            return popularsDto;
        }
        public async Task<List<ImageDto>> GetProductImages(int productId)
        {
            var images = await _repositoryManager.Image.GetProductImages(productId);
            var imagesDto = new List<ImageDto>();
            images.ForEach(async x1 => imagesDto.Add(new ImageDto
            {
                Id = x1.Id,
                ProductId = productId,
                Name = await _imageBL.GetImageOriginal(x1.Id.ToString())
            }));
            return imagesDto;
        }
        public async Task<ImageDto> GetProductImage(int imageId)
        {
            var image = await _repositoryManager.Image.GetImage(imageId , false);
            return new ImageDto
            {
                Id = imageId,
                ProductId = image.ProductId,
                Name = await _imageBL.GetImageOriginal(image.Id.ToString())
            };
        }
        //ProductType------------------------------------------------
        public async Task<List<ProductType>> GetProductTypes(string lang = "en")
        {
            var list = new List<ProductType>();
            var types = await _repositoryManager.ProductType.GetProductTypes();
            foreach (var type in types)
            {
                string typeName = "";
                if (type.Id == 1)
                { typeName = lang == "en" ? type.Type : " منتج بسيط"; }
                else if (type.Id == 2)
                { typeName = lang == "en" ? type.Type : " منتج له سمات"; }
                else 
                { typeName = lang == "en" ? type.Type : " منتج خارجي"; }

                list.Add(new ProductType
                {
                    Id = type.Id,
                    Type = typeName
                });
            }
            return list;
        }
        //AttributesProduct------------------------------------------------
        public async Task<ProductAttribut> GetProductAttribut(int id)
        {
            return await _repositoryManager.Attribute.GetAttributeId(id, false);
        }
        public async Task<List<ProductAttribut>> GetProductAttributesByProdId(int productId)
        {
            return await _repositoryManager.Attribute.GetAttributesProductId(productId);
        }
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
                var special = await IsOffer(productId);
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
        public async Task<ProductOption> GetProductOption(int optionId)
        {
            return await _repositoryManager.Option.GetOptionId(optionId, false);
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
        public async Task<List<ProductOption>> GetAllOptions()
        {
            return await _repositoryManager.Option.GetAllOptions();
        }
        public async Task<List<OptionDto>> GetProductOptions()
        {
            var options = await GetAllOptions();
            var optionsDto = _mapper.Map<List<OptionDto>>(options);
            return optionsDto;
        }
        //Value------------------------------------------------
        public async Task<List<ProductOptionValue>> GetValuesOption(int optionId)
        {
            return await _repositoryManager.Value.GetValuesOPtionId(optionId);
        } 
        public async Task<List<ProductOptionValue>> GetValues()
        {
            return await _repositoryManager.Value.GetValues();
        }
        public async Task<ProductOptionValue> FindOptionValue(int valueId)
        {
            return await _repositoryManager.Value.GetValueId(valueId, false);
        }
        public async Task<ValueDto> GetValue(int valueId)
        {
            var value = await _repositoryManager.Value.GetValueId(valueId, false);
            if (value == null) { return null; }
            else
            {
                var valueDto = _mapper.Map<ValueDto>(value);
                return valueDto;
            }
        }
        public async Task<List<ValueDto>> GetListValues(int optionId)
        {
            var value = await _repositoryManager.Value.GetValuesOPtionId(optionId);
            if (value == null) { return null; }
            else
            {
                var valueDto = _mapper.Map<List<ValueDto>>(value);
                return valueDto;
            }
        }
        public async Task AddValue(int optionId ,CreateValueDto createValueDto , string ValueHexModel = "#000000")
        {
            var option = await GetProductOption(optionId);
            if (option != null)
            {
                var value = _mapper.Map<ProductOptionValue>(createValueDto);
                value.OptionId = optionId;
                value.ValueHexModel = (option.OptionType == "radio" ? "" : ValueHexModel);
                _repositoryManager.Value.CreateValue(value);
                await _repositoryManager.SaveAsync();
            }
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
        public int ReviewsCount(int productId)
        {
            return _repositoryManager.Review.GetReviewsCount(productId);
        }
       
        public async Task<BussnessResultModel> AddReview(int productId,int customerId, CreateReviewDto createReviewDto)
        {
            if(customerId > 0)
            {
                var reviewCustomer = await _repositoryManager.Review.GetReviewProductIdToCustomerId(productId, customerId, true);
                if (reviewCustomer == null)
                {
                    var review = _mapper.Map<Review>(createReviewDto);
                    var product = await _repositoryManager.Product.GetActiveProductById(productId, true);
                    product.Rate = await Rate(productId);
                    product.CountReviews++;
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
        public async Task ActiveReview(int id)
        {
            var review = await _repositoryManager.Review.GetReviewId(id, true);
            review.IsStatus = Status.Active;
            await _repositoryManager.SaveAsync();
        }
        public async Task DeactiveReview(int id)
        {
            var review = await _repositoryManager.Review.GetReviewId(id, true);
            review.IsStatus = Status.NotActive;
            await _repositoryManager.SaveAsync();
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
            var reviewsDto = new List<ReviewDto>();
            if (reviews.Count() > 0)
            {
                foreach (var review in reviews)
                {
                    reviewsDto.Add(new ReviewDto
                    {
                        Id = review.Id,
                        Rating = Convert.ToDouble(review.Rating),
                        Text = review.Text,
                        CustomerId = review.CustomerId,
                        CustomerName = review.Customer.FullName,
                        CustomerImage = review.Customer.Avater ?? null,
                        ProductId = productId
                    });
                }
            }
            else
            {
                return null;
            }
            return reviewsDto;
        }
        public async Task<List<ReviewDto>> GetReviews(int productId)
        {
            var reviews = await _repositoryManager.Review.GetReviewsProductId(productId);
            var reviewsDto = _mapper.Map<List<ReviewDto>>(reviews);
            return reviewsDto;
        } 
        public async Task<List<ReviewDto>> GetActiveReviews(int productId)
        {
            var reviews = await _repositoryManager.Review.GetReviewsActiveProductId(productId);
            var reviewsDto = _mapper.Map<List<ReviewDto>>(reviews);
            return reviewsDto;
        } 
        public async Task<List<Review>> GetListReviews()
        {
            return await _repositoryManager.Review.GetReviews();
        }
        public async Task<Review> FindReview(int id)
        {
           return await _repositoryManager.Review.GetReviewId(id , false);
        }
        public async Task<decimal> Rate(int productId)
        {
            var reviews = await _repositoryManager.Review.GetReviewsProductId(productId);
            decimal rate = (reviews.Count() > 0 ? Convert.ToDecimal(reviews.Sum(r => r.Rating) / reviews.Count()) : 0);
            return rate;
        }
        //WishList------------------------------------------------
        public async Task<BussnessResultModel> AddWishList(CreateLikeDto create)
        {
            var like = await _repositoryManager.WishList.GetWishListProductIdCustomerId(create.CustomerId, create.ProductId);
            if (like == null)
            {
                var product = await _repositoryManager.Product.GetActiveProductById(create.ProductId, true);
                product.NumLike++;
                var wishList = _mapper.Map<WishList>(create);
                _repositoryManager.WishList.Addlike(create.ProductId, wishList);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(wishList, "successfully Added");
            }
            else
            {
                await DeleteLike(like.Id, create.CustomerId);
                return new BussnessResultModel(like, "successfully Deleted");
            }
        }
        public async Task<BussnessResultModel> DeleteLike(int id ,int customerId)
        {
            var wishList = await _repositoryManager.WishList.GetLikeCustomerId(id, customerId);
            if (wishList != null)
            {
                 var product = await _repositoryManager.Product.GetActiveProductById(wishList.ProductId, true);
                 product.NumLike--;
                _repositoryManager.WishList.DeleteLike(wishList);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(wishList, _locService.GetLocalizedStringValue("successDelete"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"),false);
            }
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
        public int GetWishList(int customerId)
        {
            return _repositoryManager.WishList.GetCountLikesByCustomersId(customerId);
        }

        //inventory------------------------------------------------
        public async Task<List<Inventory>> GetInventoryInByProductAttr(int productId, string attribute)
        {
            return await _repositoryManager.Inventory.GetProductIdOptoinIdInStock(productId, Convert.ToInt32(attribute));
        }
        public async Task<List<Inventory>> GetInventoryOutByProductAttr(int productId, string attribute)
        {
            return await _repositoryManager.Inventory.GetProductIdOptoinIdOutStock(productId, Convert.ToInt32(attribute));
        }
        public async Task<List<Inventory>> GetInStock(int productsId)
        {
            return await _repositoryManager.Inventory.GetOptionsByProductIdInStock(productsId);
        }
        public async Task<List<Inventory>> GetOutStock(int productsId)
        {
            return await _repositoryManager.Inventory.GetOptionsByProductIdOutStock(productsId);
        }
        public async Task<Inventory> GetInventoryByProduct(int productId)
        {
            return await _repositoryManager.Inventory.GetInventoryByProductId(productId);
        }
        public async Task<List<Inventory>> GetInventoryListByProduct(int productId)
        {
            return await _repositoryManager.Inventory.GetAllInventoryByPrductId(productId);
        }
     
        public async Task AddInventory(CreateInventoryDto createDto)
        {
            var inventory = _mapper.Map<Inventory>(createDto);

            var item = await GetInStock(createDto.ProductId);
            if (item != null)
            {
                await DeleteInventory(createDto.ProductId, createDto.AttributesProductId.Value);
            }
            if (createDto.AdminId != null || createDto.VendorId != null)
            {
                var product = await _repositoryManager.Product.GetActiveProductById(createDto.ProductId, true);
                product.Availability = product.Availability + createDto.Stock;
                inventory.StockType = "in";
                inventory.AddedDate = _util.EasternTime.Millisecond;
                _repositoryManager.Inventory.AddInventory(inventory);
            }
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteInventory(int productId, int attr)
        {
            var attrp = await _repositoryManager.Inventory.GetProductIdOptoinIdInStock(productId, attr);
            if (attr == null)
            {
                attrp = attrp.Where(r => r.AttributesProductId == null).ToList();
            }
            foreach (var t in attrp)
            {
                _repositoryManager.Inventory.DeleteInventory(t);
            }
            await _repositoryManager.SaveAsync();
        }
        public async Task<int> AvailabilityProducts(int productId)
        {
            int total = 0;
            var instock = 0;
            var outstock = 0;

            var inventories = await _repositoryManager.Inventory.GetAllInventoryByPrductId(productId);
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
