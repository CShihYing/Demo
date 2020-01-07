(function () {
    Vue.component('myTable',
        {
            template: ' <el-table :data="data">' +
                '<template v-for="colConfig in colConfigs">' +
                ' <slot v-if="colConfig.slot" :name="colConfig.slot"></slot>' +
                '<el-table-column v-else v-bind="colConfig"></el-table-column>' +
                '</template>  ' +
                ' </el-table>',
            props: ['colConfigs', 'data']
        });

    var app = new Vue({
        el: '#app',
        data: {
            urls: {
                savePath: window.injectObj.urls.savePath || '', //結帳
                removePath: window.injectObj.urls.removePath || '' //移除商品
            },
            shoppingCarList: window.injectObj.shoppingCarList, //購物車列表
            totalAmount:0, //總金額
            colConfigs: //表格設定
            [
                { prop: 'ProductName', label: '名稱' },
                { prop: 'Qty', label: '數量' },
                { prop: 'Price', label: '價格' },
                { slot: 'delete' }
            ]

        },
        methods: {
            //結帳
            Save: function () {
                var me = this;
                window.axios.post
                    (me.urls.savePath
                ).then(function (response) {
                    if (!response.data.Status) {
                        alert(response.data.Result);
                    } else {
                        $('#Modal-SaveSuccess').modal('show');
                    }
                    }).catch(function (response) {
                        alert('資料傳遞發生錯誤，請稍後再試！');
                    });
            },
            //刪除商品
            DeleteProduct: function (id) {
                var me = this;

                window.axios.post
                    (me.urls.removePath,
                        {
                            productId: id
                        }
                    ).then(function (response) {
                        if (!response.data.Status) {
                            alert(response.data.Result);
                        } else {
                            location.reload();
                        }
                }).catch(function (response) {
                        alert('資料傳遞發生錯誤，請稍後再試！');
                    });
            },
            //全部刪除
            DeleteAll: function (item) {
                this.DeleteProduct(-1);
            },
            //初始化
            Init: function () {
                var me = this;
                this.shoppingCarList.forEach(function(item) {
                    item.Price = item.Qty * item.Price;
                    me.totalAmount += item.Price;
                });
            }
        }
    });
    app.Init();

})();

