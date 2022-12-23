
using AutoMapper;
using BusinessLogic.ViewModel;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Models.Enums;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.ApiClasses
{
    public class ProductBL
    {
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper;
        private readonly ImageBL _imageBL;
        private readonly Util _util;
        private readonly LocService _locService;
         private readonly Language lang;

        public ProductBL(IRepositoryManager repositoryManager, IMapper mapper, ImageBL imageBL, Util util , LocService locService)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _imageBL = imageBL;
            _util = util;
            _locService = locService;
        }
        //Category------------------------------------------------
        public async Task<List<CategoryDto>> GetAllCategories()
        {
            var categories = await _repositoryManager.Categories.GetAllCategories(false);
            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories);
            var category = categories.First();
            var categoryDto = categoriesDto.First();
            categoryDto.CategoryName = lang.Code == "en" ? category.CategoryName : category.CategoryNameAr;
            return categoriesDto;
        }
        public async Task<CategoryDto> GetCategory(int catId)
        {
            var category = await _repositoryManager.Categories.GetCategoryById(catId ,false);
            var categoryDto = _mapper.Map<CategoryDto>(category);
            categoryDto.CategoryName = lang.Code == "en" ? category.CategoryName : category.CategoryNameAr;
            return categoryDto;
        }
        public async Task<List<CategoryDto>> GetSearchMainCategories(string search)
        {
            var categories = await _repositoryManager.Categories.SearchMainCategoriesCP(search);
            var mainCategoryDto = _mapper.Map<List<CategoryDto>>(categories);
            return mainCategoryDto;
        }
        public async Task<IEnumerable<Category>> SubCategoriesMainId()
        {
            return await _repositoryManager.Categories.SubCategoriesMainId();
        }
        public async Task<IEnumerable<Category>> GetSubActiveCategories()
        {
            return await _repositoryManager.Categories.GetSubActiveCategories(false);
        }
        public async Task<List<CategoryDto>> GetMainCategories()
        {
            var categories = await _repositoryManager.Categories.GetSubCategories(false);
            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories);
            var category = categories.First();
            var categoryDto = categoriesDto.First();
            categoryDto.CategoryName = lang.Code == "en" ? category.CategoryName : category.CategoryNameAr;
            return categoriesDto;
        }
        public async Task<List<CategoryDto>> GetSubCategoriesCP(int mainId)
        {
            var categories = await _repositoryManager.Categories.GetSubCategoriesMainIDCP(mainId);
            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories);
            var category = categories.First();
            var categoryDto = categoriesDto.First();
            categoryDto.CategoryName = lang.Code == "en" ? category.CategoryName : category.CategoryNameAr;
            return categoriesDto;
        }
        public async Task CreateCategory(CreateCategoryDto createCategoryDto)
        {
            var category = _mapper.Map<Category>(createCategoryDto);
            if(createCategoryDto.MainCategoryId == null)
            {
                category.MainCategoryId = 0;
            }
            else
            {
                category.MainCategoryId = createCategoryDto.MainCategoryId.Value;
            }
            _repositoryManager.Categories.CreateMainCategory(category);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteCategory(int id)
        {
            var MainCategory = await _repositoryManager.Categories.GetCategoryById(id, false);
            if(MainCategory == null)
            {
                var subCategoryList = await _repositoryManager.Categories.GetSubCategoriesByMainId(id, false);
                if (subCategoryList != null)
                {
                    foreach (var item in subCategoryList)
                    {
                        var products = await _repositoryManager.Product.GetProductsCatId(item.Id);
                        if (products != null)
                        {
                            foreach (var product in products)
                            {
                                await RemoveProduct(product.Id);
                            }
                        }
                        _repositoryManager.Categories.DeleteCategory(item);
                    }
                }
                _repositoryManager.Categories.DeleteCategory(MainCategory);
            }
            await _repositoryManager.SaveAsync();
        }
        public async Task EditCategory (UpdateCategoryDto updateDto)
        {
            var category = await _repositoryManager.Categories.GetCategoryById(updateDto.Id, true);
            if(category != null)
            {
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
            }
        }

        //Product------------------------------------------------
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
        public async Task<BussnessResultModel> AddProduct(int catId, CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);
            product.CategoryId = catId;
            if (createProductDto.StoreId != null)
            {
                product.StoreId = createProductDto.StoreId;
                product.IsAcceptAdmin = false;
            }
            else
            {
                product.AdminId = createProductDto.AdminId;
                product.IsAcceptAdmin = true;
            }
            _repositoryManager.Product.AddProductOnCategory(catId, product);
            await _repositoryManager.SaveAsync();

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
            if (product.IsSale == true)
            {
                var craete = createProductDto.ProductSales.First();
               
                var sales = _mapper.Map<ProductSales>(craete);
                _repositoryManager.Sales.AddFlashSale(sales);
            }
            if (product.IsSpecial == true)
            {
                var special = _mapper.Map<SpecialProducts>(createProductDto.SpecialProducts.First());
                _repositoryManager.SpecialProducts.AddSpecialProduct(special);
            }
            if (product.Availability != 0)
            {
                var createInventory = new CreateInventoryDto();
                var inventory = _mapper.Map<Inventory>(createInventory);
                inventory.Stock = product.Availability;
                inventory.StockType = "in";
                //inventory.AddedDate = _orderBL.EasternTime.Millisecond;
                inventory.VendorId = product.StoreId == 0 ? 0 : product.StoreId;
               _repositoryManager.Inventory.AddInventory(inventory);
            }
            return new BussnessResultModel(product);
        }
        public async Task EditProduct(int catId ,UpdateProductDto updateDto)
        {
            var product = await _repositoryManager.Product.GetProductById(updateDto.Id, true);
            product.CategoryId = catId;
            if (updateDto.StoreId != null)
            {
                product.StoreId = updateDto.StoreId;
                product.IsAcceptAdmin = false;
            }
            else
            {
                product.AdminId = updateDto.AdminId;
                product.IsAcceptAdmin = true;
            }
            _mapper.Map(updateDto, product);
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
            if (product.IsSale == true )
            {
                var sales = _mapper.Map<ProductSales>(updateDto.ProductSales.First());
                _repositoryManager.Sales.AddFlashSale(sales);
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
                var special = _mapper.Map<SpecialProducts>(updateDto.SpecialProducts.First());
                _repositoryManager.SpecialProducts.AddSpecialProduct(special);
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
        }
        public async Task ApproveProduct(int productId)
        {
            var product = await _repositoryManager.Product.GetProductById(productId, true);
            product.IsAcceptAdmin = true;
            await _repositoryManager.SaveAsync();
        }
        public async Task<Product> CheckApproveProduct(int productId)
        {
          return await _repositoryManager.Product.CheckApproveProduct(productId);
        } 
        public async Task<List<OrderProduct>> GetOrdersProduct(int productId)
        {
          return await _repositoryManager.OrderProducts.GetOrdersProductId(productId);
        }
        public async Task RemoveProduct(int productId)
        {
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
            }
            await _repositoryManager.SaveAsync();
        }
        public async Task<List<ProductVM>> GetProducts()
        {
            var products = await _repositoryManager.Product.GetAllAcceptedProducts();
            var productsDto = new List<ProductVM>();
            
            if (products != null)
            {
                foreach (var product in products)
                {
                    var category = await _repositoryManager.Categories.GetCategoryToPrductId(product.Id);
                    var special = await IsOffer(product.Id);
                    var flash = await _repositoryManager.Sales.GetFlashProductId(product.Id);
                    // var customerId = product.CustomerProducts.First().CustomerId;
                    if (product != null)
                    {
                        productsDto.Add(new ProductVM
                        {
                            MainCategoryId = (category != null ? category.MainCategoryId : 0),
                            CategoryId = (category != null ? category.Id : 0),
                            CategoryName = (category == null ? null : category.CategoryName),
                           // CategoryImage = (category != null ? await _imageBL.GetImageThumbnail(category.ImgId.ToString()) : ""),

                            Id = product.Id,
                            ProductName = product.ProductName,
                            Description = product.Description,
                            ProductModel = product.ProductModel,
                            //ProductImage = await _imageBL.GetImageThumbnail(product.Images.First().Id.ToString()),
                            TypeId = product.TypeId,
                            Price = product.Price,
                            IsStatus = product.IsStatus.ToString(),
                            Availability = await AvailabilityProducts(product.Id),
                            AttributesProducts = await GetAttributsProducts(product.Id),
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


                            //IsFavorite =   product.IsFavorite ,// await IsFavourite(customerId.Value, product.Id),
                            NumLike = product.NumLike, //await GetFavourite(customerId.Value, product.Id),
                            //IsReview = await IsReview(customerId.Value, product.Id),
                            Reviews = await GetReviews(product.Id),
                            Rate = await Rate(product.Id),

                            StoreId = product.StoreId,
                            StoreName = product.StoreId == null ? null : product.Store.FullName,
                            StoreImage = product.StoreId == null ? null : product.Store.Avater
                            
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
        public async Task<List<ProductDto>> GetProductsCP(int storeId , string search , int catId , int customerId , string Lang)
        {
            var products = await _repositoryManager.Product.GetProductByCategoryAndStoreIdForCP(storeId, search, catId);
            var productsDto =_mapper.Map<List<ProductDto>>(products);
            return productsDto;
        }
        public async Task<ProductVM> GetProduct(int productId, int customerId)
        {
            var special = await IsOffer(productId);
            var flash = await _repositoryManager.Sales.GetFlashProductId(productId);
            var category = await _repositoryManager.Categories.GetCategoryToPrductId(productId);
            var product = await _repositoryManager.Product.GetAcceptAdminActiveProduct(productId);
            var store = await _repositoryManager.User.GetStoreId(product.StoreId.Value);
            if (store != null)
            {
                product.StoreId = store.Id;
            }

            return product == null ? null : new ProductVM
            {
                MainCategoryId = (category != null ? category.MainCategoryId : 0),
                CategoryId = (category != null ? category.Id : 0),
                CategoryName = (category == null ? null : category.CategoryName),
                // CategoryImage = (category != null ? await _imageBL.GetImageThumbnail(category.ImgId.ToString()) : ""),

                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                ProductModel = product.ProductModel,
                ProductImage = await _imageBL.GetImageThumbnail(product.Images.First().Id.ToString()),
                TypeId = product.TypeId,
                Price = product.Price,
                IsStatus = product.IsStatus.ToString(),
                Availability = await AvailabilityProducts(product.Id),
                AttributesProducts = await GetAttributsProducts(product.Id),
                Images = await _imageBL.GetListImagesProductIdAsync(product.Id),
                ShareLink = _util.url1 + "/en/Home/share?id=" + product.Id,
                //storeUrl + "/en/Home/share?id=" + item.products_id + "&name=" + (item.ProductNames != null ? item.ProductNames[lange].Replace(" ", "").Trim() : ""),
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

                StoreId = product.StoreId,
                StoreName = product.StoreId == null ? null : store.FullName,
                StoreImage = product.StoreId == null ? null : store.Avater
            };
        }
        public async Task<List<ProductVM>> GetProductsCatId(int catId)
        {
            var products = await GetProducts();
            var productsCatId = products.Where(c => c.CategoryId == catId).ToList();
            return productsCatId;
        }
        public async Task<List<ProductVM>> GetSpecialsProd()
        {
            var products = await GetProducts();
            var productsCatId = products.Where(c => c.IsSpecial == true).ToList();
            return productsCatId;
        }
        public async Task<List<ProductVM>> GetFlashProds()
        {
            var products = await GetProducts();
            var productsCatId = products.Where(c => c.IsSale == true).ToList();
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
            var imagesDto = new ImageDto();
            return image == null ? null : new ImageDto
            {
                Id = imageId,
                ProductId = image.ProductId,
                Name = await _imageBL.GetImageOriginal(image.Id.ToString())
            };
            return imagesDto;
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
        public async Task DeleteAttribute(int id)
        {
            var attribut = await _repositoryManager.Attribute.GetAttributeId(id, false);
            _repositoryManager.Attribute.DeleteAttributesProduct(attribut);
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateAttribute(UpdateAttributeDto update)
        {
            var attribut = await _repositoryManager.Attribute.GetAttributeId(update.Id, true);
            _mapper.Map(update, attribut);
            await _repositoryManager.SaveAsync();
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
        public async Task AddOption(CreateOptionDto createOptionDto)
        {
            var option = _mapper.Map<ProductOption>(createOptionDto);
            _repositoryManager.Option.CreateOption(option);
            await _repositoryManager.SaveAsync();
        }
        public async Task EditOption(UpdateOptionDto update)
        {
            var option =  await _repositoryManager.Option.GetOptionId(update.Id, true);
            if(option != null)
            {
                _mapper.Map(update, option);
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task DeleteOptionProduct(int id)
        {
            var option = await _repositoryManager.Option.GetOptionId(id, false);
            if( option != null)
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
        public async Task<List<ProductOption>> GetAllOptions()
        {
            return await _repositoryManager.Option.GetAllOptions();
        } 
        public async Task<List<OptionDto>> GetOptions()
        {
            var options = await GetAllOptions();
            var optionsDto =  _mapper.Map<List<OptionDto>>(options);
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
        public async Task DeleteValueProduct(int valueId)
        {
            var value = await _repositoryManager.Value.GetValueId(valueId, false);
            _repositoryManager.Value.DeleteValue(value);
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateValue(UpdateValueDto updateDto)
        {
            var value = await _repositoryManager.Value.GetValueId(updateDto.Id, true);
            _mapper.Map(updateDto, value);
            await _repositoryManager.SaveAsync();
        }
        //Review------------------------------------------------
        public int ReviewsCount(int productId)
        {
            return _repositoryManager.Review.GetReviewsCount(productId);
        }
        public async Task<Review> GetReviewProductAndCustomer(int CustomerId, int productId)
        {
            return await _repositoryManager.Review.GetReviewProductIdToCustomerId(productId, CustomerId);
        }
        public async Task AddReviews(int productId, CreateReviewDto createReviewDto)
        {
            var review = _mapper.Map<Review>(createReviewDto);
            var product = await _repositoryManager.Product.GetActiveProductById(productId, true);
            product.Rate = await Rate(productId);
            product.CountReviews++;
            
            review.ProductId = productId;
            review.IsStatus = Status.NotActive;
            _repositoryManager.Review.AddReview(review);
            await _repositoryManager.SaveAsync();
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
        public async Task AddWishList( CreateLikeDto createLikeDto)
        {
            var product = await _repositoryManager.Product.GetActiveProductById(createLikeDto.ProductId , true);
            product.NumLike++;
            var wishList = _mapper.Map<WishList>(createLikeDto);
            _repositoryManager.WishList.Addlike(createLikeDto.ProductId, wishList);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteLike(int customerId, int productId)
        {
            var product = await _repositoryManager.Product.GetActiveProductById(productId, true);
            product.NumLike--;
            var wishList = await _repositoryManager.WishList.GetWishListProductIdCustomerId(customerId , productId);
            if (wishList != null)
            {
                _repositoryManager.WishList.DeleteLike(wishList);
                await _repositoryManager.SaveAsync();
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

            inventory.VendorId = 1;
            if (inventory.AdminId != null || inventory.VendorId != null)
            {
                var product = await _repositoryManager.Product.GetActiveProductById(createDto.ProductId, true);
                product.Availability = product.Availability + createDto.Stock;
                inventory.StockType = "in";
               // inventory.AddedDate = _orderBL.EasternTime.Millisecond;
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
