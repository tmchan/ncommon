SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [MonthlySalesSummary](
	[Year] [int] NOT NULL,
	[Month] [int] NOT NULL,
	[SalesPersonId] [int] NOT NULL,
	[Amount] [decimal](19, 5) NULL,
	[Currency] [nvarchar](255) NULL,
	[SalesPersonFirstName] [nvarchar](255) NULL,
	[SalesPersonLastName] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Year] ASC,
	[Month] ASC,
	[SalesPersonId] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Customers](
	[CustomerID] [int] IDENTITY(1,1) NOT NULL,
	[StreetAddress1] [nvarchar](255) NULL,
	[StreetAddress2] [nvarchar](255) NULL,
	[City] [nvarchar](255) NULL,
	[State] [nvarchar](255) NULL,
	[ZipCode] [nvarchar](255) NULL,
	[FirstName] [nvarchar](255) NULL,
	[LastName] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Products](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Orders](
	[OrderID] [int] IDENTITY(1,1) NOT NULL,
	[OrderDate] [datetime] NULL,
	[ShipDate] [datetime] NULL,
	[CustomerId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [OrderItems](
	[OrderItemID] [int] IDENTITY(1,1) NOT NULL,
	[Price] [decimal](19, 5) NULL,
	[Quantity] [int] NULL,
	[Store] [nvarchar](255) NULL,
	[ProductId] [int] NULL,
	[OrderId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderItemID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [OrderItems]  WITH CHECK ADD  CONSTRAINT [FK_OrderItems_Product] FOREIGN KEY([ProductId])
REFERENCES [Products] ([ProductID])
GO
ALTER TABLE [OrderItems] CHECK CONSTRAINT [FK_OrderItems_Product]
GO
ALTER TABLE [OrderItems]  WITH CHECK ADD  CONSTRAINT [FK_Orders_OrderItems] FOREIGN KEY([OrderId])
REFERENCES [Orders] ([OrderID])
GO
ALTER TABLE [OrderItems] CHECK CONSTRAINT [FK_Orders_OrderItems]
GO
ALTER TABLE [Orders]  WITH CHECK ADD  CONSTRAINT [FK_Customer_Orders] FOREIGN KEY([CustomerId])
REFERENCES [Customers] ([CustomerID])
GO
ALTER TABLE [Orders] CHECK CONSTRAINT [FK_Customer_Orders]
GO
