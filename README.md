# TechMarkt - Tech Shopping Web Application

A modern e-commerce platform designed specifically for technology products, built with ASP.NET Core MVC and featuring comprehensive admin management capabilities.

## 🎯 Purpose

This document defines the functional and non-functional requirements for the TechMarkt shopping web application. It serves as a guide for developers, stakeholders, and testers to ensure alignment on project scope, features, and technical specifications.

## 🔍 Project Scope

The TechMarkt application provides:

- **User Experience**: Browse, search, and purchase tech products with an intuitive interface
- **Admin Management**: Comprehensive admin panel for managing products, users, and orders
- **Security**: Secure authentication and payment processing integration
- **Responsiveness**: Optimized experience across desktop, tablet, and mobile devices

## 🛠️ Technologies Used

### Backend
- **Framework**: ASP.NET Core MVC
- **ORM**: Entity Framework Core
- **Database**: Microsoft SQL Server with Code-First Migration
- **Authentication**: ASP.NET Identity (Role-based)

### Frontend
- **Views**: ASP.NET MVC Razor Views
- **Styling**: HTML5, CSS3, Bootstrap 5
- **Interactivity**: JavaScript
- **Responsive Design**: Bootstrap responsive grid system

### Integration & Tools
- **API**: RESTful Web Services
- **Development**: Visual Studio
- **Project Management**: Trello
- **Communication**: Slack

## 🏗️ System Architecture

The application is a standalone e-commerce platform that integrates with RESTful APIs for product data management. It follows the MVC architectural pattern with Entity Framework for database operations and MSSQL for data persistence.

## ✨ Key Features

### 🔐 User Authentication
- Secure user registration and login system
- Password recovery functionality
- Role-based access control using ASP.NET Identity

### 🛍️ Product Browsing
- Browse products by category and price range
- Advanced search with keyword filtering
- Real-time filtering capabilities
- Product favorites system

### 👨‍💼 Admin Dashboard
- **Product Management**: Complete CRUD operations for products and categories
- **User Management**: View user activities and manage user access
- **Order Management**: Track and update order statuses
- **Content Management**: Manage homepage sliders and promotional content

### 🛒 Cart & Checkout
- Add/remove items with quantity adjustment
- Secure checkout process
- Payment integration (Stripe-ready)
- Order confirmation system

### 📦 Order Tracking
- Real-time order status updates
- Status progression: Processing → Shipped → Delivered
- Order history for users

### 📱 Responsive Design
- Mobile-first approach using Bootstrap 5
- Optimized UI for all device sizes
- Touch-friendly interface elements

### 📊 Sales Analytics
- Generate comprehensive sales reports
- Revenue trend tracking
- User activity monitoring
- Performance metrics dashboard

## 🎭 User Roles & Capabilities

### Customer Features
- Product browsing with advanced filters
- Shopping cart management
- Secure checkout process
- User account management
- Order tracking and history
- Product favorites list

### Admin Features
- Complete product catalog management
- User account oversight and management
- Order processing and status updates
- Sales analytics and reporting
- Content management (sliders, promotions)
- System monitoring capabilities

### 🗂️ System Diagrams

![Use Case Diagram](.wwwroot/img/UseCaseDiagram2.png)



**UML Class Diagram**: *[Add your UML class diagram here showing system classes and relationships]*

**Entity Relationship Diagram**: *[Add your ER diagram here showing database schema]*

**Architectural Diagram**: *[Add your system architecture diagram here showing application layers]*

