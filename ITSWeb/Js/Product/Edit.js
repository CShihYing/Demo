(function () {


    var app = new Vue({
        el: '#app',
        data: {
            urls: {
                savePath: window.injectObj.urls.savePath || '' //存檔路徑
            },
            name: window.injectObj.product.ProductName, //名稱
            quantity: window.injectObj.product.ProductQuantity, //數量
            price: window.injectObj.product.ProductPrice, //價格
            status: window.injectObj.product.IsShow, //上下架狀態
            filePath: window.injectObj.product.ProductImage, //圖案路徑
            file: '', //檔案 
            fileShow: false //是否顯示縮圖
        },
        methods: {
            //儲存
            Save: function () {
                var me = this;

                var formData = new FormData();
                formData.append('ProductName', me.name);
                formData.append('ProductQuantity', me.quantity);
                formData.append('ProductPrice', me.price);
                formData.append('IsShow', me.status);
                formData.append('ProductId', window.injectObj.product.ProductId);
                formData.append('ImageFile', me.file);

                window.axios.post
                (me.urls.savePath,
                    formData
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
            //更改上/下架狀態
            ChangeStatus: function () {
                this.status = !this.status;
            },
            //選擇檔案上傳
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
            //初始化
            Init: function() {
                if (this.filePath !== '') {
                    $("#previewImage").attr('src', this.filePath);
                    this.fileShow = true;
                }
            }
        }
    });
    app.Init();
})();