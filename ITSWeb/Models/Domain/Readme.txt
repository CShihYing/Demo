資料層。
i. 以Model為主資料夾，主目錄下存放Entity Framework物件，並且以來源資料庫名稱為主；由系統自動產生的Entity Model（DB Model）不可任意手動修改。
ii. 若有自行定義的模組物件（Domain Model），可於其下建置Class子目錄，命名方式：{模組名稱}+DomainModel.cs。
iii. 請注意，模組（Model）中僅可建立屬性（Property），不允許建立任何方法（Method）。