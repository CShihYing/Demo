介面（Interface）
1.使用Pascal-Case命名風格。
2.使用名詞、名詞片語或形容詞片語來命名。
3.介面應以前綴字”I”來表示，像是IHostingModule、IDataFactory、IConnectionProvider等。
4.介面所定義的任何屬性、方法或事件命名與類別相同。

抽象類別（Abstract Class）
1.與一般類別的命名相同
2.於抽象類別的方法，應該是以其衍生類別必須要實作的方法為主，而基底類別或一般類別若有預期其衍生類別會改變原有方法的行為時，請務必將它宣告為虛擬方法（virtual method）。