# Blood-Donation-Management-System

A MVC (.NET 7.0) web application made for university course project Database Systems. The project contains the following features and technologies that are used:

### Technology
#### Front-End:
* HTML 5
* CSS
* JavaScript
* JQuery

#### Back-End:
  * MVC Entity FrameWork Core (C#)
  * Microsoft SQL Server 2022 (For Database)

### Features (Key functionalities)
1) The application supports three different types of users:
  * Donor 
  * Employee
  * Organization
#### Organization
  * The Organization can register themselves. After that they can login and add employees as well as bloodcamps on different locations (stored in database) and they can select one bloodcamp per location.
  * The Organization can add employees and assign them the bloodcamp in which they will work.
  * The organization can perform CRUD operations on bloodcamps and employees and can update its own details as well.
#### Donor
  * The Donor can register themselves and then can book an appointment at any of the available bloodcamps.
  * The Donor will then get the timings of the appointment on the dashboard screen.
  * After donation the donor can see also his/her results.
#### Employees
  * The employee will login using the credentials given by the organization.
  * The employee can see all the pending tests of the donors it has on the bloodcamp assigned to the employee.
  * The employee can test and set the report for the donor.
 
