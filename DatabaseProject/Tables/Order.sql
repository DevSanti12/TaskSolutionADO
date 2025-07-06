CREATE TABLE [dbo].[Order]
(
	[Id] INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	Status NVARCHAR(50) NOT NULL,
	CreatedDate DATETIME NOT NULL,
	UpdatedDate DATETIME NOT NULL,
	ProductId INT NOT NULL,
	CONSTRAINT FK_Product FOREIGN KEY (ProductId) references [dbo].[Product](Id)
)
