(function () {
    Vue.component('my-table',
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
                savePath: window.injectObj.urls.savePath || '',
                removePath: window.injectObj.urls.removePath || ''
            },
            shoppingCarList: window.injectObj.shoppingCarList,
            totalAmount:0,
            colConfigs:
            [
                { prop: 'ProductName', label: '名稱' },
                { prop: 'Qty', label: '價格' },
                { prop: 'Price', label: '數量' },
                { slot: 'delete' }
            ]

        },
        methods: {
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
            DeleteAll: function (item) {
                this.DeleteProduct(-1);
            },
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

