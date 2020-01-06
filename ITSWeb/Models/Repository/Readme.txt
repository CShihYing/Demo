資料存取層：
i. 以Repository為主資料夾，其下存放Repository Pattern及Unit of Work Pattern實作類別，分別為GenericRepository.cs及UnitOfWork.cs。
ii. 若有實作類別，可於其下建置Class子目錄（可依模組或功能，再細分下層目錄），命名方式：{Table名稱}+Repository.cs，與資料庫Table為一對一的關係。