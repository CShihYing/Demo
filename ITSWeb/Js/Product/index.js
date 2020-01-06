(function () {


    var app = new Vue({
        el: '#app',
        data: {
            urls: {
                addShoppingCarPath: window.injectObj.urls.addShoppingCarPath || ''
            },
            productList: window.injectObj.productList
        },
        methods: {
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
            onFileChange: function (event) {
                var file = event.target.files[0];
                var me = this;
                if (file && file !== 'undefined') {
                    me.file = file;
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        $("#previewImage").attr('src', e.target.result);
                    }
                    reader.readAsDataURL(file);
                    me.fileShow = true;
                } else {
                    me.file = '';
                    me.fileShow = false;
                }
            },
            Init: function () {
                this.productList.forEach(function (item) {
                    item.ProductQuantity = 1;
                });
                
            }
        }
    });
    app.Init();
})();