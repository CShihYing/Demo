(function () {


    var app = new Vue({
        el: '#app',
        data: {
            urls: {
                savePath: window.injectObj.urls.savePath || '' //新增路徑
            },
            name: '', //名稱
            quantity: '', //數量
            price: 0,//價格
            status: true, //上下架
            file: '', //檔案
            fileShow:false //是否顯示縮圖
        },
        methods: {
            //新增
            Save: function () {
                var me = this;
                
                var formData = new FormData();
                formData.append('ProductName', me.name);
                formData.append('ProductQuantity', me.quantity);
                formData.append('ProductPrice', me.price);
                formData.append('IsShow', me.status);
                formData.append('ImageFile', me.file);
                
                window.axios.post
                (me.urls.savePath,
                 formData
                ).then(function (response) {
                    if (response.data.Status) {
                        $('#Modal-AddSuccess').modal('show');
                    } else {
                        alert(response.data.Result);
                    }
                }).catch(function (response) {
                    alert('資料傳遞發生錯誤，請稍後再試！');
                });
            },
            //改變上/下架
            ChangeStatus: function () {
                this.status = !this.status;
            },
            //選擇檔案上傳
            onFileChange: function(event) {
                var file = event.target.files[0];
                var me = this;
                if (file && file !== 'undefined') {
                    me.file = file;
                    var reader = new FileReader();
                    reader.onload = function(e) {
                        $("#previewImage").attr('src', e.target.result);
                    }
                    reader.readAsDataURL(file);
                    me.fileShow = true;
                } else {
                    me.file = '';
                    me.fileShow = false;
                }
            }
        }
    });

})();