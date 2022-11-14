
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

        public ProductBL(IRepositoryManager repositoryManager, IMapper mapper, UserBL userBL, LocService locService, ImageBL imageBL, Util util)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _userBL = userBL;
            _locService = locService;
            _imageBL = imageBL;
            _util = util;
        }
        //Category------------------------------------------------
        public async Task<List<CategoryDto>> GetAllCategories()
        {
            var categories = await _repositoryManager.Categories.GetCategoriesWithMainCategories(false);
            var categoryDto = _mapper.Map<List<CategoryDto>>(categories);
            return categoryDto;
        }
      
        public async Task<List<MainCategoryDto>> GetMainCategories()
        {
            var categories = await _repositoryManager.Categories.GetAllCategories(false);
            var mainCategoryDto = _mapper.Map<List<MainCategoryDto>>(categories);
            return mainCategoryDto;
        }
        public async Task CreateMainCategory(CreateCategoryDto createCategoryDto)
        {
            var category = _mapper.Map<Category>(createCategoryDto);
            _repositoryManager.Categories.CreateMainCategory(category);
            await _repositoryManager.SaveAsync();
        }
        public async Task CreateSupCategory(int mainId, CreateCategoryDto createCategoryDto)
        {
            var category = _mapper.Map<Category>(createCategoryDto);
            category.MainCategoryId = mainId;
            _repositoryManager.Categories.CreateSubCategory(mainId, category);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteCategory(int id)
        {
            var category = await _repositoryManager.Categories.GetCategoryById(id, false);
            _repositoryManager.Categories.DeleteCategory(category);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteMainCategory(int id)
        {
            var subCategoryList = await _repositoryManager.Categories.GetSubCategoriesByMainId(id, false);
            if (subCategoryList != null)
            {
                foreach (var item in subCategoryList)
                {
                    _repositoryManager.Categories.DeleteCategory(item);
                }
                await _repositoryManager.SaveAsync();
            }
            var MainCategory = await _repositoryManager.Categories.GetCategoryById(id, false);
            _repositoryManager.Categories.DeleteCategory(MainCategory);
            await _repositoryManager.SaveAsync();
        }

        //Product------------------------------------------------

        public async Task<List<ProductVM>> GetProductByModel(List<int> prodList, int CustomerId, Currency curr)
        {
            if (prodList.Count() > 0)
            {
                var productModel = new List<ProductVM>();
                foreach (var id in prodList)
                {
                    var product = await _repositoryManager.Product.GetAcceptAdminActiveProduct(id);
                    if (product != null)
                    {
                        var store = await _userBL.GetStore(product.ProductsStores.First().VendorId);
                        var category = await _repositoryManager.Categories.GetCategoryToPrductId(id);
                        var special = await IsOffer(id);
                        var flash = await _repositoryManager.Sales.GetFlashProductId(id);
                        var specialPrice = special.Id == 0 ? 0 : special.SpecialPrice;
                        productModel.Add(new ProductVM
                        {
                            MainCategoryId = (category != null ? category.MainCategoryId : 0),
                            CategoryId = (category != null ? category.Id : 0),
                            CategoryName = (category != null ? category.CategoryName : ""),
                            CategoryImage = (category != null ? await _imageBL.GetImageThumbnail(category.ImgId.ToString()) : ""),
                           
                            Id = id,
                            ProductName = product.ProductName,
                            Description = product.Description,
                            ProductModel = product.ProductModel,
                            TypeId = product.TypeId,
                            ProductPrice = product.Price,
                            ProductStatus = product.IsStatus.ToString(),
                            ProductImage = await _imageBL.GetImageThumbnail(product.Images.First().Id.ToString()),
                            images = _imageBL.GetListImagesProductId(id),
                            AvailabilityProduct = await AvailabilityProducts(id),
                            ShareLink = _util.url1 + "/share.html?id=" + id,
                            Options = await GetOptions(id),

                            is_special = (special.Id == 0 ? false : true),
                            isFlash = (flash != null ? true : false),
                            offer_price = specialPrice,
                            flash_price = (flash != null ? flash.DiscountPrice : 0), 
                            startDate = (flash != null ? flash.StartDate : null),
                            expireDate = (flash != null ? flash.EndDate : null),

                            IsBest = Convert.ToInt16(product.IsBest),
                            IsFeature = product.IsFeature,

                            IsFav = (CustomerId == 0 ? false : await IsFavourite(CustomerId, id)),
                            likeId = await GetFavourite(CustomerId, id),
                            IsReview = (CustomerId == 0 ? false : await IsReview(id, CustomerId)),
                            Reviews = await GetReviews(id),
                            Rate = await Rate(id),

                            StoreId = product.ProductsStores.First().VendorId,
                            StoreName = store.FirstName + " " + store.LastName,
                            StoreImage = store.ImageId.ToString()
                        });
                    }
                }
                foreach (var item in productModel)
                {
                    if (item.isFlash == true)
                    {
                        item.ProductPrice = item.flash_price;
                    }
                }
                foreach (var item in productModel)
                {
                    if (item.is_special == true)
                    {
                        item.offer_price = item.offer_price;
                    }
                }
                return productModel;
            }
            else
            {
                return new List<ProductVM>();
            }
        }
        public async Task<List<ProductVM>> GetProductsCatId(int catId, int CustomerId, Currency curr)
        {
            List<int> ids = new List<int>();
            var products = _repositoryManager.Product.GetAllProducts().Where(c => c.IsAcceptAdmin == true).Select(c => c.Id);
            if (catId != 0)
            {
               products = await _repositoryManager.Product.GetAllProductsToCategoryId(catId);
            }
             ids = products.ToList();
            return await GetProductByModel(ids, CustomerId, curr);
        }
       
        public List<Product> GetProducts()
        {
            return  _repositoryManager.Product.GetAllProducts();
        }
        public async Task<List<ProductDto>> GetProductEasys(int storeId, string search, int catId, int CustomerId, string lange)
        {
            var products = await _repositoryManager.Product.SearshProductByCategoryAndStore(storeId, search, catId);
            var productsDto = _mapper.Map<List<ProductDto>>(products);
            var product = products.FirstOrDefault();
            var imageId = product.Images.First().Id;
            foreach (var productDto in productsDto)
            {
                productDto.ProductName = (product.ProductName != null ? product.ProductName = _locService.GetLocalizedStringValue(lange) : "");
                productDto.Description = (product.Description != null ? product.Description = _locService.GetLocalizedStringValue(lange) : "");
                productDto.Availability = await AvailabilityProducts(product.Id);
              //  productDto.Images.Select(c => c.Id = Convert.ToInt32(url + "/" + filesRootPath + _imageBL.GetImageOriginal(imageId.ToString())));
                productDto.Rate = await Rate(product.Id);
                productDto.IsFavorite = await IsFavourite(CustomerId, product.Id);
                productDto.Reviews = await GetLast3Reviews(product.Id);
                productDto.IsSpecial = IsOffer(product.Id).Id != 0;
                //productDto.ProductSales. = await _repositoryManager.Sales.GetFlashProductId(product.Id);
            }
            return productsDto;
        }
        public async Task<List<Product>> GetProductsByVendor(int vendorId)
        {
            return await _repositoryManager.Product.GetProductsTOStoreId(vendorId);
        }
        public async Task AddProduct(int catId, CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);
            _repositoryManager.Product.AddProductOnCategory(catId, product);
            await _repositoryManager.SaveAsync();
        }
        public async Task EditProduct(int productId, UpdateProductDto updateProductDto)
        {
            var product = await _repositoryManager.Product.GetProductById(productId, true);
            _mapper.Map(updateProductDto, product);
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
            var customerProducts = await _repositoryManager.CustomerProduct.GetCustomersProductId(productId);
            foreach (var customerProduct in customerProducts)
            {
                _repositoryManager.CustomerProduct.DeleteCustomerProduct(customerProduct);
                await _repositoryManager.SaveAsync();
            }
            var stores = await _repositoryManager.ProductStore.GetAllProductsStoreProductId(productId);
            foreach (var store in stores)
            {
                _repositoryManager.ProductStore.DeleteProductsStore(store);
                await _repositoryManager.SaveAsync();
            }
            var carts = await _repositoryManager.CartProduct.GetAllCartProductProductId(productId);
            foreach (var cart in carts)
            {
                _repositoryManager.CartProduct.DeleteCartProduct(cart);
                await _repositoryManager.SaveAsync();
            }

            var specials = await _repositoryManager.SpecialProducts.GetSpecialProductsProductId(productId);
            foreach (var special in specials)
            {
                _repositoryManager.SpecialProducts.DeleteSpecialProduct(special);
                await _repositoryManager.SaveAsync();
            }

            var sales = await _repositoryManager.Sales.GetAllSalesProductId(productId);
            foreach (var sale in sales)
            {
                _repositoryManager.Sales.DeleteFlashSale(sale);
                await _repositoryManager.SaveAsync();
            }
            var likes = await _repositoryManager.WishList.GetLikesProductId(productId);
            foreach (var like in likes)
            {
                _repositoryManager.WishList.DeleteLike(like);
                await _repositoryManager.SaveAsync();
            }

            var product = await _repositoryManager.Product.GetProductById(productId, false);
            _repositoryManager.Product.DeleteProduct(product);
            await _repositoryManager.SaveAsync();
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
            var popular = await _repositoryManager.Product.GetLatestPage(pageSize);
            var popularDto = _mapper.Map<List<ProductPageDto>>(popular);
            return popularDto;
        }
        public async Task<List<ProductPageDto>> SpecialsPage(int pageSize = 5)
        {
            var popular = await _repositoryManager.Product.SpecialsPage(pageSize);
            var popularDto = _mapper.Map<List<ProductPageDto>>(popular);
            return popularDto;
        }

        public async Task<List<ProductPageDto>> TopRatedPage(int pageSize = 6)
        {
            var populars = await _repositoryManager.Product.TopRatedPage(pageSize);
            var popular = populars.First();
            var popularsDto = _mapper.Map<List<ProductPageDto>>(populars);
            var popularDto = popularsDto.First();
            popularDto.Rate = await Rate(popular.Id);
            return popularsDto;
        }
        public async Task<List<ProductPageDto>> DailyDeals()
        {
            var populars = await _repositoryManager.Product.DailyDeals();
            var popular = populars.First();
            var popularsDto = _mapper.Map<List<ProductPageDto>>(populars);
            var popularDto = popularsDto.First();
            popularDto.Rate = await Rate(popular.Id);
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
        public async Task DeleteAttributesProduct(int id, int productId)
        {
            var attributId = await _repositoryManager.Attribute.GetAttributeIdProductId(id, productId);
            _repositoryManager.Attribute.DeleteAttributesProduct(attributId);
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateAttributells(int attributId, UpdateAttributeDto updateOptionDto)
        {
            var attribut = await _repositoryManager.Attribute.GetAttributeId(attributId, true);
            _mapper.Map(updateOptionDto, attribut);
            await _repositoryManager.SaveAsync();
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
            var attributId = await _repositoryManager.Option.GetOptionId(id, false);
            _repositoryManager.Option.DeleteOption(attributId);
            await _repositoryManager.SaveAsync();
        }
        public async Task<string> GetPriceForOption(int id, string option = "")
        {
            decimal productPrice = 0;
            int stock = 0;
            var attrProduct = await _repositoryManager.Attribute.GetAttributeId(id , false);
            if (attrProduct != null)
            {
                int prodId = attrProduct.ProductId;
                var product = await _repositoryManager.Product.GetProductById(prodId , false);
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
                var inStockList = await _repositoryManager.Inventory.GetProductIdOptoinIdInStock(prodId,Convert.ToInt32(option));
                var OutStockList = await _repositoryManager.Inventory.GetProductIdOptoinIdOutStock(prodId, Convert.ToInt32(option));
                stock = inStockList.Sum(r => r.Stock) - OutStockList.Sum(r => r.Stock);
            }
            return productPrice + "_" + stock;
        }
        //OptionValue------------------------------------------------
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
        public async Task AddValue(CreateValueDto createValueDto)
        {
            var value = _mapper.Map<ProductOptionValue>(createValueDto);
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
        //SpecialProducts------------------------------------------------
        public async Task<List<ProductVM>> GetSpecialsProd(int CustomerId, Currency curr)
        {
            var items = _repositoryManager.SpecialProducts.GetSpecialProducts().Select(r => r.Id);
            List<int> ids = items.ToList();
            return await GetProductByModel(ids, CustomerId, curr);
        }
        public async Task AddSpecialProducts(CreateSpecialProductsDto createSpecialProductsDto)
        {
            var specialProducts = _mapper.Map<SpecialProducts>(createSpecialProductsDto);
            _repositoryManager.SpecialProducts.AddSpecialProduct(specialProducts);
            await _repositoryManager.SaveAsync();
        }
        public async Task<SpecialProducts> IsOffer(int productId)
        {
            var special = new SpecialProducts();
            var specialProducts = await _repositoryManager.SpecialProducts.GetSpecialProductId(productId);
            if (specialProducts != null)
            {
                special.Id = specialProducts.Id;
                special.ProductId = productId;
                special.EndDate = specialProducts.EndDate;
                special.IsStatus = specialProducts.IsStatus;
                special.SpecialPrice = specialProducts.SpecialPrice;
            }
            return special;
        }
        public async Task DeleteSpecialProduct(int productId)
        {
            var specialProducts = await _repositoryManager.SpecialProducts.GetSpecialProductId(productId);
            if (specialProducts != null)
            {
                _repositoryManager.SpecialProducts.DeleteSpecialProduct(specialProducts);
            }
            await _repositoryManager.SaveAsync();
        }
        public async Task EditSpecialProduct(int productId, UpdateSpecialProductDto updateProductDto)
        {
            var specialProducts = await _repositoryManager.SpecialProducts.CheckSpecialExists(productId, true);
            _mapper.Map(updateProductDto, specialProducts);
            await _repositoryManager.SaveAsync();
        }
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
                var orderAttributProducts = await _repositoryManager.OrderAttributesProducts.GetAttributesOrderProduct(orderId ,productId);
                if (orderAttributProducts.Count() > 0)
                {
                    foreach (var item in orderAttributProducts)
                    {
                        var option = await _repositoryManager.Option.GetOptionId(item.ProductAttribut.OptionId , false);
                        var values = orderAttributProducts.Where(r => r.ProductAttribut.OptionId == option.Id).ToList();
                        if (values.Count() > 0)
                        {
                            foreach (var itemValue in values)
                            {
                                var value = await _repositoryManager.Value.GetValueId(itemValue.ProductAttribut.ValueId , false);
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
                        var attributProduct = await _repositoryManager.Attribute.GetProductOptionValue(productId , attr.OptionId, attr.ValueId);

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
        public async Task<List<OptionDto>> GetOptions(int productId, int langId = 1)
        {
            var lst = await _repositoryManager.Attribute.GetAttributesProductId(productId);
            var attrs = lst.GroupBy(x => x.OptionId).Select(x => x.First()).ToList();
            var opt = new List<OptionDto>();

            if (attrs.Count() > 0)
            {
                foreach (var t in attrs)
                {
                    var vs = new List<valus>();
                    var a = await _repositoryManager.Option.GetOptionId(t.OptionId, false);
                    var values = lst.Where(r => r.OptionId == a.Id).ToList();
                    if (values.Count() > 0)
                    {
                        foreach (var v in values)
                        {

                            var val = await _repositoryManager.Value.GetValueId(t.ValueId ,false);
                            vs.Add(new valus
                            {
                                IsDefault = ((short)(v.IsDefault == 0 ? 0 : 1)),
                                OptionValueName = a.OptionName,
                                OptionName = val.OptionValueName,
                                ValueHexModel = val.ValueHexModel,
                                OptionId = val.OptionId,
                                ValueId = val.Id,
                                option_attribute_id = v.Id,
                                AttributePrice = (v.PricePrefix == "+" ? v.AttributePrice : -v.AttributePrice)
                            });
                        }
                    }
                    opt.Add(new OptionDto
                    {
                        OptionName = a.OptionName,
                        Id = a.Id,
                        OptionType = a.OptionType,
                        Values = vs,
                    });

                }
            }

            return opt;
        }
        //salesProduct------------------------------------------------
        public async Task<List<ProductVM>> GetFlashProds(int CustomerId, Currency curr)
        {
            var items = _repositoryManager.Sales.GetAllSales().Select(r => r.Id);
            List<int> ids = items.ToList();
            return await GetProductByModel(ids, CustomerId, curr);
        }
        public async Task AddFlashSale(CreateProductSalesDto createProductSalesDto)
        {
            var sales = _mapper.Map<ProductSales>(createProductSalesDto);
            _repositoryManager.Sales.AddFlashSale(sales);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteFlashSale(int productId)
        {
            var salesProduct = await _repositoryManager.Sales.CheckFlashExists(productId, false);
            if (salesProduct != null)
            {
                _repositoryManager.Sales.DeleteFlashSale(salesProduct);
            }
            await _repositoryManager.SaveAsync();
        }
        public async Task EditSaleProduct(int productId, UpdateSalesProductDto updateProductDto)
        {
            var product = await _repositoryManager.Sales.CheckFlashExists(productId, true);
            _mapper.Map(updateProductDto, product);
            await _repositoryManager.SaveAsync();
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
        public async Task UpdateCustomerProduct( int productId , UpdateCustomerProductDto customerProductDto)
        {
            var customerId = 0;//GetCurrentUserId();
            var customerProduct = await _repositoryManager.CustomerProduct.GetCustomerIdProduct(productId, customerId);
            customerProduct.CustomerId = customerId;
            _mapper.Map(customerProductDto, customerProduct);
            customerProduct.FinalPrice = customerProductDto.FinalPrice * customerProduct.Quantity;
            customerProduct.UpdatedAt = DateTime.UtcNow;
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateAmountCart (int customerId, int qty, int productId, string lange)
        {
            var customerProduct = await _repositoryManager.CustomerProduct.GetCustomerIdProduct(productId, customerId);
             var customerProductDto = _mapper.Map<UpdateCustomerProductDto>(customerProduct);
            if (customerProduct != null)
            {
                var product = await _repositoryManager.Product.GetProductById(productId , false);
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
        public async Task DeleteCustomerStoreCart(int customerId , int storeId)
        {
            var storesCustomers = await _repositoryManager.CustomerProduct.GetStoreCustomer(customerId , storeId);
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
            //review.CustomerId = GetCurrentUserId();
            review.ProductId = productId;
            _repositoryManager.Review.AddReview(review);
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateReviews(int reviewId, UpdateReviewDto updateReviewDto)
        {
            var review = await _repositoryManager.Review.GetReviewId(reviewId, true);
            _mapper.Map(updateReviewDto, review);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteReviews(int productId, int reviewId)
        {
            var review = await _repositoryManager.Review.GetReviewId(reviewId, false);
            review.ProductId = productId;
            _repositoryManager.Review.DeleteReview(review);
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
            var reviewsDto = _mapper.Map<List<ReviewDto>>(reviews);
            return reviewsDto;
        }
        //public async Task<List<ReviewDto>> GetPaginationReviews(int productId, PostsParameters postsParameters)
        //{
        //    var reviews = await _repositoryManager.Review.GetReviewsByProductId(productId, postsParameters);
        //    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(reviews.MetaData));
        //    var reviewsDto = _mapper.Map<List<ReviewDto>>(reviews);
        //    return reviewsDto;
        //}
        public async Task<List<ReviewDto>> GetReviews(int productId)
        {
            var reviewsDto = new List<ReviewDto>();
            var reviews = await _repositoryManager.Review.GetReviewsProductId(productId);
            if (reviews.Count() > 0)
            {
                foreach (var review in reviews)
                {
                    reviewsDto.Add(new ReviewDto
                    {
                        CustomerId = review.CustomerId,
                        CustomerName = review.Customer.FirstName + " "+ review.Customer.LastName,
                        CustomerImage = review.Customer.Avater ?? null ,
                        ProductId = review.ProductId,
                        Id = review.Id,
                        Rating = Convert.ToDouble(review.Rating),
                        Text = review.Text
                    });
                }
            }
            return reviewsDto;
        }
        public async Task<decimal> Rate(int productId)
        {
            var reviews = await _repositoryManager.Review.GetReviewsProductId(productId);
            reviews.Where(c => c.IsStatus == Status.Active);
            decimal rate = (reviews.Count() > 0 ? Convert.ToDecimal(reviews.Sum(r => r.Rating) / reviews.Count()) : 0);
            return rate;
        }
        //WishList------------------------------------------------
        public async Task<WishList> GetLikeUserToProduct(int user, int productId)
        {
            return await _repositoryManager.WishList.GetWishListProductIdCustomerId(user, productId);
        }

        public int GetLikeCountToUser(int user)
        {
            return _repositoryManager.WishList.GetCountLikesByCustomersId(user);
        }
        public async Task AddWishList(int productId, int userId, CreateLikeDto createLikeDto)
        {
            var wishList = _mapper.Map<WishList>(createLikeDto);
            wishList.CustomerId = userId;
            wishList.ProductId = productId;
            _repositoryManager.WishList.Addlike(productId, wishList);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteLikeCustomerProduct(int customerId, int productId)
        {
            var wishList = await GetLikeUserToProduct(customerId, productId);
            _repositoryManager.WishList.DeleteLike(wishList);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteLikeCustomer(int id, int customerId)
        {
            var wishList = await _repositoryManager.WishList.GetLikeCustomerId(id, customerId);
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
    }
}
