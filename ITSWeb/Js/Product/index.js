(function () {


    var app = new Vue({
        el: '#app',
        data: {
            urls: {
                addShoppingCarPath: window.injectObj.urls.addShoppingCarPath || '' //加入購物車
            },
            productList: window.injectObj.productList //商品列表
        },
        methods: {
            //新增
            Add: function (item) {
                var me = this;
                window.axios.post
                    (me.urls.addShoppingCarPath,
                    {
                        Id: item.ProductId,
                        Qty: item.ProductQuantity,
                        Price: item.ProductPrice,
                        ProductName: item.ProductName
                    }
                ).then(function (response) {
                    if (response.data.Status) {
                        $('#Modal-EditSuccess').modal('show');
                    } else {
                        alert(response.data.Result);
                    }
                }).catch(function (response) {
                    alert('資料傳遞發生錯誤，請稍後再試！');
                });
            },
            //初始化
            Init: function () {
                this.productList.forEach(function (item) {
                    item.ProductQuantity = 1;
                });
                
            }
        }
    });
    app.Init();
})();