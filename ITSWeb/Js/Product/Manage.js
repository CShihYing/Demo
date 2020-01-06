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
                searchPath: window.injectObj.urls.searchPath || '',
                deletePath: window.injectObj.urls.deletePath || '',
                editPath: window.injectObj.urls.editPath || '',
                addPath: window.injectObj.urls.addPath || ''
            },
            filter: {
                keyword:''
            },
            tableData: window.injectObj.productList,
            colConfigs: 
                [
                    { prop: 'ProductName', label: '名稱' },
                    { prop: 'ProductPrice', label: '價格' },
                    { prop: 'ProductQuantity', label: '剩餘數量' },
                    { slot: 'delete' },
                    { slot: 'edit' }
                ]
            
        },
        methods: {
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
            ChangeStatus: function () {
                this.status = !this.status;
            },
            EditProduct: function (item) {
                var me = this;
                window.location.href = me.urls.editPath + '?id=' + item.ProductId;
            },
            Add: function() {
                var me = this;
                window.location.href = me.urls.addPath ;
            },
            DeleteProduct: function(item) {
                var me = this;

                window.axios.post
                    (me.urls.deletePath,
                    {
                        id:item.ProductId
                    }
                ).then(function (response) {
                    me.Search();
                }).catch(function (response) {
                    alert('資料傳遞發生錯誤，請稍後再試！');
                });
            }
        }
    });

   
})();

