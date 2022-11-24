
using AutoMapper;
using BusinessLogic.ViewModel;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Models.Enums;
using Entities.RequestFeatures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace BusinessLogic.ApiClasses
{
    public class ProductBL
    {
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper;
        private readonly UserBL _userBL;
        private readonly ImageBL _imageBL;
        private readonly LocService _locService;
        private readonly Util _util;
        private readonly OrderBL _orderBL;
        private readonly ImageUploadServices _imageUploadServices;
        public ProductBL(IRepositoryManager repositoryManager, IMapper mapper, UserBL userBL, LocService locService, ImageBL imageBL, Util util , OrderBL orderBL  ,ImageUploadServices imageUploadServices)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _userBL = userBL;
            _locService = locService;
            _imageBL = imageBL;
            _util = util;
            _orderBL = orderBL;
            _imageUploadServices = imageUploadServices;
        }
        //Category------------------------------------------------
        public async Task<List<CategoryDto>> GetAllCategories()
        {
            var categories = await _repositoryManager.Categories.GetAllCategories(false);
            var categoryDto = _mapper.Map<List<CategoryDto>>(categories);
            return categoryDto;
        }
        public async Task<List<MainCategoryDto>> GetMainCategories()
        {
            var categories = await _repositoryManager.Categories.GetMainCategories(false);
            var mainCategoryDto = _mapper.Map<List<MainCategoryDto>>(categories);
            return mainCategoryDto;
        }
        public async Task<List<CategoryDto>> GetSubCategories()
        {
            var categories = await _repositoryManager.Categories.GetSubCategories(false);
            var mainCategoryDto = _mapper.Map<List<CategoryDto>>(categories);
            return mainCategoryDto;
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
            var MainCategory = await _repositoryManager.Categories.GetCategoryById(id, false);
            _repositoryManager.Categories.DeleteCategory(MainCategory);
            await _repositoryManager.SaveAsync();
        }
        public async Task EditCategory (int id, UpdateCategoryDto updateDto)
        {
            var category = await _repositoryManager.Categories.GetCategoryById(id, true);
            _mapper.Map(updateDto, category);
            await _repositoryManager.SaveAsync();
        }

        //Product------------------------------------------------
        public async Task AddProduct(int catId, CreateProductDto createProductDto)
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
            //if (createProductDto.Images != null)
            //{
            //    var createImageDto = new CreateImageDto
            //    {
            //        //Name = ,
            //        ProductId = product.Id
            //    };
            //    await _imageBL.AddImage(createImageDto);
            //}
            if (product.IsSale == true)
            {
                var sales = _mapper.Map<ProductSales>(createProductDto.ProductSales.First());
                _repositoryManager.Sales.AddFlashSale(sales);
            }
            if (product.IsSpecial == true)
            {
                var special = _mapper.Map<SpecialProducts>(createProductDto.SpecialProducts.First());
                _repositoryManager.SpecialProducts.AddSpecialProduct(special);
            }
        }
        public async Task EditProduct(int productId, UpdateProductDto updateProductDto)
        {
            var product = await _repositoryManager.Product.GetProductById(productId, true);
            if (updateProductDto.StoreId != null)
            {
                product.StoreId = updateProductDto.StoreId;
                product.IsAcceptAdmin = false;
            }
            else
            {
                product.AdminId = updateProductDto.AdminId;
                product.IsAcceptAdmin = true;
            }
            _mapper.Map(updateProductDto, product);
            //if (updateProductDto.Images != null)
            //{
            //    var fileName = updateProductDto.Images.First().Name;
            //    var createImageDto = new CreateImageDto
            //    {
            //        Name = fileName,
            //        ProductId = productId
            //    };
            //    await _imageBL.AddImage(createImageDto);
            //}
            if (product.IsSale == true )
            {
                var sales = _mapper.Map<ProductSales>(updateProductDto.ProductSales.First());
                _repositoryManager.Sales.AddFlashSale(sales);
            }
            else 
            {
                var salesProduct = await _repositoryManager.Sales.CheckFlashExists(productId, false);
                if (salesProduct != null)
                {
                    _repositoryManager.Sales.DeleteFlashSale(salesProduct);
                }
            }
            if (product.IsSpecial == true )
            {
                var special = _mapper.Map<SpecialProducts>(updateProductDto.SpecialProducts.First());
                _repositoryManager.SpecialProducts.AddSpecialProduct(special);
            }
            else
            {
                var specials = await _repositoryManager.SpecialProducts.GetSpecialProductsProductId(productId);
                if (specials != null)
                {
                    foreach (var special in specials)
                    {
                        _repositoryManager.SpecialProducts.DeleteSpecialProduct(special);
                    }
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
        public async Task RemoveProduct(int productId)
        {
            var product = await _repositoryManager.Product.GetProductById(productId, false);
            if (product != null)
            {
                var carts = await _repositoryManager.CartProduct.GetAllCartProductProductId(productId);
                if (carts != null)
                {
                    foreach (var cart in carts)
                    {
                        _repositoryManager.CartProduct.DeleteCartProduct(cart);
                    }
                }
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
            //var productsCatId = products.Where(c => c.CategoryId == catId);
            
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
        //ProductType------------------------------------------------
        public async Task<List<ProductType>> GetProductTypes(string lang = "en")
        {
            var list = new List<ProductType>();
            var types = await _repositoryManager.ProductType.GetProductTypes();
            foreach (var type in types)
            {
                string typeName = "";
                if (type.Id == 0)
                { typeName = lang == "en" ? type.Type : " منتج بسيط"; }
                else if (type.Id == 1)
                { typeName = lang == "en" ? type.Type : " منتج له سمات"; }
                else if (type.Id == 2)
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
        public async Task AddAttribute (int productId ,CreateAttributeDto createDto)
        {
            var attribute = _mapper.Map<ProductAttribut>(createDto);
            attribute.ProductId = productId;
            _repositoryManager.Attribute.AddAttributesProduct(productId , attribute);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteAttribute(int id, int productId)
        {
            var attributId = await _repositoryManager.Attribute.GetAttributeIdProductId(id, productId);
            _repositoryManager.Attribute.DeleteAttributesProduct(attributId);
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateAttribute(int attributId, UpdateAttributeDto updateOptionDto)
        {
            var attribut = await _repositoryManager.Attribute.GetAttributeId(attributId, true);
            _mapper.Map(updateOptionDto, attribut);
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
                var inStockList = await _repositoryManager.Inventory.GetProductIdOptoinIdInStock(prodId, attrProduct.OptionId);
                var OutStockList = await _repositoryManager.Inventory.GetProductIdOptoinIdOutStock(prodId, attrProduct.OptionId);
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
        public async Task DeleteOptionProduct(int id)
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
            var option = await _repositoryManager.Option.GetOptionId(id, false);
            _repositoryManager.Option.DeleteOption(option);
            await _repositoryManager.SaveAsync();
        }
        //test not finish
        public async Task<decimal> getOptionsOrdersTotalPrice(int productId, int orderId)
        {
            decimal tot = 0;
            var orderProduct = await _repositoryManager.OrderProducts.GetOrderProductsId(productId, orderId);
            if (orderProduct != null)
            {
                tot = orderProduct.FinalPrice;
                var special = await IsOffer(productId);
                if (special != null && special.Id != 0)
                {
                    tot = special.SpecialPrice;
                }
                var orderAttributProducts = await _repositoryManager.OrderAttributesProducts.GetAttributesOrderProduct(orderId, productId);
                if (orderAttributProducts.Count() > 0)
                {
                    foreach (var item in orderAttributProducts)
                    {
                        var option = await _repositoryManager.Option.GetOptionId(item.ProductAttribut.OptionId, false);
                        var values = orderAttributProducts.Where(r => r.ProductAttribut.OptionId == option.Id).ToList();
                        if (values.Count() > 0)
                        {
                            foreach (var itemValue in values)
                            {
                                var value = await _repositoryManager.Value.GetValueId(itemValue.ProductAttribut.ValueId, false);
                                if (value != null)
                                {
                                    var valueDto = _mapper.Map<ValueDto>(value);
                                    valueDto.OptionValueName = (option.OptionType == "color" ? value.ValueHexModel : value.OptionValueName);
                                }

                                if (itemValue.ProductAttribut.PricePrefix == "+")
                                {
                                    tot += Convert.ToDecimal(itemValue.ProductAttribut.AttributePrice);
                                }

                                if (itemValue.ProductAttribut.PricePrefix == "-")
                                {
                                    if (tot != 0)
                                    {
                                        tot -= Convert.ToDecimal(itemValue.ProductAttribut.AttributePrice);
                                    }
                                }
                            }
                            _mapper.Map<OptionDto>(option);
                        }
                    }
                }
            }
            return tot;
        }
        public async Task<decimal> getOptionsOrdersTotalPriceTest(int productId, int orderId)
        {
            decimal total = 0;
            var orderProduct = await _repositoryManager.OrderProducts.GetOrderProductsId(productId, orderId);
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
                        var attributProduct = await _repositoryManager.Attribute.GetProductOptionValue(productId, attr.OptionId, attr.ValueId);

                        if (attr.PricePrefix == "+")
                        {
                            total += Convert.ToDecimal(item.ProductAttribut.AttributePrice);
                        }

                        if (attr.PricePrefix == "-")
                        {
                            if (total != 0)
                            {
                                total -= Convert.ToDecimal(item.ProductAttribut.AttributePrice);
                            }
                        }
                    }
                }
            }
            return total;
        }
        public async Task<List<OptionDto>> GetOptionsCart(int cartProductId)
        {
            var cartAttributeProducts = await _repositoryManager.CartAttributeProduct.CartAttributeProducts(cartProductId);
            var optionDto = new List<OptionDto>();
            if (cartAttributeProducts.Count() > 0)
            {
                foreach (var item in cartAttributeProducts)
                {
                    var option = await _repositoryManager.Option.GetAllOptions();
                    var productAttribut = await _repositoryManager.Attribute.GetProductOptionValue(item.CartProduct.ProductId, item.AttributesProduct.OptionId, item.AttributesProduct.ValueId);
                    if (productAttribut != null)
                    {
                        var value = await _repositoryManager.Value.GetValueId(item.AttributesProduct.ValueId, false);
                        var attributeDto = _mapper.Map<AttributeDto>(productAttribut);
                        attributeDto.AttributePrice = (productAttribut.PricePrefix == "+" ? productAttribut.AttributePrice : -productAttribut.AttributePrice);
                    }
                    optionDto = _mapper.Map<List<OptionDto>>(option);
                }
            }

            return optionDto;
        }
        //Value------------------------------------------------
        public async Task<List<ProductOptionValue>> GetValuesOption(int optionId)
        {
            return await _repositoryManager.Value.GetValuesOPtionId(optionId);
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
        public async Task AddValue(int optionId ,CreateValueDto createValueDto)
        {
            var value = _mapper.Map<ProductOptionValue>(createValueDto);
            value.OptionId = optionId;
            _repositoryManager.Value.CreateValue(value);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteValueProduct(int valueId)
        {
            var value = await _repositoryManager.Value.GetValueId(valueId, false);
            _repositoryManager.Value.DeleteValue(value);
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateValue(int valueId, UpdateValueDto updateOptionDto)
        {
            var value = await _repositoryManager.Value.GetValueId(valueId, true);
            _mapper.Map(updateOptionDto, value);
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
        //CustomerProduct------------------------------------------------
        public async Task AddCustomerProduct(CreateCustomerProductDto createDto, int customerId, int productId)
        {
            var customerProduct = _mapper.Map<CustomerProduct>(createDto);
            customerProduct.CustomerId = customerId;
            customerProduct.ProductId = productId;

            var attributs = await _repositoryManager.Attribute.GetAttributesProductId(productId);
            var createAttributeDtos = createDto.CustomerAttributesProducts;
            if (createAttributeDtos != null && createAttributeDtos.Count() > 0)
            {
                foreach (var item in createAttributeDtos)
                {
                    var attribut = attributs.Where(r => r.Id == item.AttributesProductId).FirstOrDefault();
                    if (attribut != null)
                    {
                        var customerAttributesProduct = _mapper.Map<CustomerAttributesProduct>(item);
                        _repositoryManager.CustomerAttributesProduct.AddAttributeCustomerProduct(customerAttributesProduct);
                        await _repositoryManager.SaveAsync();
                    }
                }
            }
            _repositoryManager.CustomerProduct.AddCustomerProduct(customerProduct);
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateCustomerProduct(int productId, UpdateCustomerProductDto customerProductDto)
        {
            var customerId = 0;//GetCurrentUserId();
            var customerProduct = await _repositoryManager.CustomerProduct.GetCustomerIdProduct(productId, customerId);
            customerProduct.CustomerId = customerId;
            _mapper.Map(customerProductDto, customerProduct);
            customerProduct.FinalPrice = customerProductDto.FinalPrice * customerProduct.Quantity;
            customerProduct.UpdatedAt = DateTime.UtcNow;
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateAmountCart(int customerId, int qty, int productId, string lange)
        {
            var customerProduct = await _repositoryManager.CustomerProduct.GetCustomerIdProduct(productId, customerId);
            var customerProductDto = _mapper.Map<UpdateCustomerProductDto>(customerProduct);
            if (customerProduct != null)
            {
                var product = await _repositoryManager.Product.GetProductById(productId, false);
                if (product != null)
                {
                    customerProductDto.FinalPrice = product.Price;
                    var specialProduct = await IsOffer(productId);
                    if (specialProduct != null)
                    {
                        customerProductDto.FinalPrice = specialProduct.SpecialPrice;
                    }
                    var attributesProducts = await _repositoryManager.CustomerAttributesProduct.GetAllAttributesCustomerProduct(customerProduct.Id);
                    if (attributesProducts.Count() > 0)
                    {
                        foreach (var item in attributesProducts)
                        {
                            var attribute = item.AttributesProduct;
                            var productOptionValue = await _repositoryManager.Attribute.GetProductOptionValue(productId, attribute.OptionId, attribute.ValueId);
                            if (productOptionValue != null)
                            {
                                if (productOptionValue.PricePrefix == "+")
                                {
                                    customerProductDto.FinalPrice += productOptionValue.AttributePrice;
                                }

                                if (productOptionValue.PricePrefix == "-" && customerProductDto.FinalPrice != 0)
                                {
                                    customerProductDto.FinalPrice -= productOptionValue.AttributePrice;
                                }
                            }
                        }
                    }
                    await UpdateCustomerProduct(productId, customerProductDto);
                }
            }
        }
        public async Task<decimal> UpdatePriceAttributesProduct(int id, UpdateCustomerProductDto customerProductDto)
        {
            var attributeList = await _repositoryManager.CustomerAttributesProduct.GetAllAttributesCustomerProduct(id);
            foreach (var attribute in attributeList)
            {
                var productAttribut = await _repositoryManager.Attribute.GetProductOptionValue(attribute.CustomerProduct.ProductId.Value, attribute.AttributesProduct.OptionId, attribute.AttributesProduct.ValueId);
                if (productAttribut != null && productAttribut.AttributePrice != 0)
                {
                    if (productAttribut.PricePrefix == "+")
                    {
                        customerProductDto.FinalPrice += productAttribut.AttributePrice;
                    }
                    if (productAttribut.PricePrefix == "-")
                    {
                        if (customerProductDto.FinalPrice != 0)
                        {
                            customerProductDto.FinalPrice -= productAttribut.AttributePrice;
                        }
                    }
                }
            }

            var customerProduct = await _repositoryManager.CustomerProduct.GetCustomerProductId(id, true);
            if (customerProduct != null)
            {
                _mapper.Map(customerProductDto, customerProduct);
                customerProduct.FinalPrice = customerProductDto.FinalPrice * customerProduct.Quantity;
                await _repositoryManager.SaveAsync();
            }
            decimal AllTotal = Convert.ToDecimal(customerProductDto.FinalPrice * customerProductDto.Quantity);

            return AllTotal;
        }
        public async Task DeleteCustomerProduct(int id)
        {
            var customerProduct = await _repositoryManager.CustomerProduct.GetCustomerProductId(id, false);
            if (customerProduct != null)
            {
                var attributeList = await _repositoryManager.CustomerAttributesProduct.GetAllAttributesCustomerProduct(id);
                if (attributeList.Count() > 0)
                {
                    foreach (var attribute in attributeList)
                    {
                        _repositoryManager.CustomerAttributesProduct.DeleteAttributeCustomerProduct(attribute);
                        await _repositoryManager.SaveAsync();
                    }
                }
                _repositoryManager.CustomerProduct.DeleteCustomerProduct(customerProduct);
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task DeleteCustomerStoreCart(int customerId, int storeId)
        {
            var storesCustomers = await _repositoryManager.CustomerProduct.GetStoreCustomer(customerId, storeId);
            foreach (var storeCustomer in storesCustomers)
            {
                _repositoryManager.CustomerProduct.DeleteCustomerProduct(storeCustomer);
                await _repositoryManager.SaveAsync();
            }
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
            if (inventory.AdminId != null || inventory.VendorId != null) 
            {
                var product = await _repositoryManager.Product.GetActiveProductById(createDto.ProductId, true);
                product.Availability = product.Availability + createDto.Stock;
                inventory.StockType = "in";
                inventory.AddedDate = _orderBL.EasternTime.Millisecond;
                _repositoryManager.Inventory.AddInventory(inventory);
            }
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateInventory(UpdateInventoryDto updateDto)
        {
            var product = await _repositoryManager.Product.GetActiveProductById(updateDto.ProductId, true);
            var inventories = await _repositoryManager.Inventory.GetProductIdOptoinIdInStock(updateDto.ProductId, updateDto.AttributesProductId.Value);
            if (inventories == null)
            {
                inventories = await _repositoryManager.Inventory.GetOptionsByProductIdInStock(updateDto.ProductId);
            }
            foreach (var inventory in inventories)
            {
                product.Availability = product.Availability - updateDto.Stock;
                inventory.StockType = "out";
                // inventory.TotalPurchasedPrice = ; //final price order
                // inventory.PurchaseCode = ; //orderId
                inventory.AddedDate = _orderBL.EasternTime.Millisecond;
                // add out inventory .
            }
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteInventory(int productId)
        {
            var product = await _repositoryManager.Product.GetActiveProductById(productId, true);
            var inventories = await _repositoryManager.Inventory.GetAllInventoryByPrductId(productId);
            if (inventories != null)
            {
                foreach(var inventory in inventories)
                {
                    product.Availability = product.Availability - inventory.Stock;
                    _repositoryManager.Inventory.DeleteInventory(inventory);
                }
                await _repositoryManager.SaveAsync();
            }
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
