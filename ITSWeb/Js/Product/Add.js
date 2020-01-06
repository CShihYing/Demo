(function () {


    var app = new Vue({
        el: '#app',
        data: {
            urls: {
                savePath: window.injectObj.urls.savePath || ''
            },
            name: '',
            quantity: '',
            price: 0,
            status: true,
            file: '',
            fileShow:false
        },
        methods: {
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
            ChangeStatus: function () {
                this.status = !this.status;
            },
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