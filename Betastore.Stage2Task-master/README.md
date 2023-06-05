# Betastore.Stage2Task
All necessary operations have been mocked using seed

Introduction: 
The purpose of this document is to outline the business requirements for the development of an e-commerce platform with discount functionality. The platform aims to provide personalized discount offers to customers based on their duration of membership and total amount spent. The system will be implemented as a REST API using C# and will utilize a local or in-memory database for storage.
Features: The following features are required to be implemented in the e-commerce platform:
User Management: 
1.1 User Registration: 
Allow users to register by providing their email, password, first name, and last name. Validate the registration data and store the user information securely. 1.2 User Login: Provide a login functionality for registered users to access the platform. Authenticate user credentials and generate a secure access token for authorization.

Customer Profile Management:
2.1 Customer Information: 
Allow users to provide additional information such as phone number and address to complete their customer profile. Enable users to update their profile information as needed.

Discount Profile Management: 
3.1 Create Discount Profiles: Allow platform administrators to create discount profiles. Each discount profile should have a name, discount type, qualification requirement, and discount percentage. The discount type can be based on criteria such as the duration of membership or the minimum amount spent. 3.2 Update/Delete Discount Profiles: Enable platform administrators to update or delete existing discount profiles.

Item Management: 
4.1 Add Items: Allow platform administrators to add new items to the platform. Each item should have a name, description, price, and stock quantity. 4.2 Update/Delete Items: Enable platform administrators to update or delete existing items. Administrators can modify the item's name, description, price, or stock quantity.

Buying Process: 
5.1 Add Items to Cart: Allow customers to add items to their shopping cart. Include options to specify the quantity of each item. 5.2 Update/Remove Items from Cart: Enable customers to update the quantity or remove items from their shopping cart. 5.3 Checkout Process: Provide a checkout process for customers to review their order details. Calculate the total amount to be paid based on the selected items and quantities.

Payment Integration: 
6.1 Payment Methods: Integrate with a payment gateway to handle customer payments securely. Support multiple payment methods such as credit cards, PayPal, etc. 6.2 Process Payment: Process customer payments securely through the integrated payment gateway. Provide a confirmation to the customer upon successful payment.

Discount Calculation and Application: 
7.1 Determine Applicable Discount: Evaluate customer profiles to determine the best applicable discount offer based on their membership duration and total amount spent. Identify the highest applicable discount based on the defined discount profiles. 7.2 Apply Discount: Apply the discount percentage to the customer's purchase automatically during the checkout process.

Database Entities and Properties:
User:
•	Id (primary key)
•	Email
•	Password
•	FirstName
•	LastName
Customer:
•	Id (foreign key referencing User.Id)
•	Phone
•	Address
DiscountProfile:
•	Id (primary key)
•	Name
•	Type
•	Requirement
•	Percentage
Item:
•	Id (primary key)
•	Name
•	Description
•	Price
•	StockQuantity
Cart:
•	Id (primary key)
•	UserId (foreign key referencing User.Id)
CartItem:
•	Id (primary key)
•	CartId (foreign key referencing Cart.Id)
•	ItemId (foreign key referencing Item.Id)
•	Quantity
CustomerPurchase:
•	Id (primary key)
•	OrderDate
•	Status
•	UserId (foreign key referencing User.Id)
PurchaseItem:
•	Id (primary key)
•	CustomerPurchaseId (foreign key referencing CustomerPurchase.Id)
•	ItemId (foreign key referencing Item.Id)
•	Quantity
•	UnitPrice
Conclusion: This document outlines the key features, user stories, and database entities for the development of an e-commerce platform with discount functionality. It provides a clear understanding of the requirements and serves as a robust reference for the product and development team.
Please note that this document provides a high-level overview, and additional detailed specifications and design considerations may be required during the development process.
If you have any further questions or need additional information, please let me know.

