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
                searchPath: window.injectObj.urls.searchPath || '', //存檔路徑
                deletePath: window.injectObj.urls.deletePath || '', //刪除路徑
                editPath: window.injectObj.urls.editPath || '', //修改路徑
                addPath: window.injectObj.urls.addPath || '' //新增路徑
            },
            filter: {
                keyword:'' //關鍵字
            },
            tableData: window.injectObj.productList, //表格內容
            deleteProductId:'', //欲刪除商品Id
            colConfigs:  //表格設定
                [
                    { prop: 'ProductName', label: '名稱' },
                    { prop: 'ProductPrice', label: '價格' },
                    { prop: 'ProductQuantity', label: '剩餘數量' },
                    { slot: 'delete' },
                    { slot: 'edit' }
                ]
            
        },
        methods: {
            //搜尋
            Search: function () {
                var me = this;

                window.axios.post
                    (me.urls.searchPath,
                    {
                        OnlyIsShow: false,
                        Key:me.filter.keyword
                    }
                ).then(function (response) {
                    if (response) {
                        me.tableData = response.data;
                    } else {
                        alert(response.data.Result);
                    }
                }).catch(function (response) {
                    alert('資料傳遞發生錯誤，請稍後再試！');
                });
            },
            //編輯
            EditProduct: function (item) {
                var me = this;
                window.location.href = me.urls.editPath + '?id=' + item.ProductId;
            },
            //新增
            Add: function() {
                var me = this;
                window.location.href = me.urls.addPath ;
            },
            //改變欲刪除商品
            ChangeDeleteProduct: function(item) {
                this.deleteProductId = item.ProductId;
                $('#Modal-Delete').modal('show');
            },
            //刪除
            DeleteProduct: function(item) {
                var me = this;

                window.axios.post
                    (me.urls.deletePath,
                    {
                        id: me.deleteProductId
                    }
                ).then(function (response) {
                    if (response.data.Status) {
                        me.Search();
                    } else {
                        alert(response.data.Result);
                    }
                }).catch(function (response) {
                    alert('資料傳遞發生錯誤，請稍後再試！');
                });
            }
        }
    });

   
})();

