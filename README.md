Truck Hire
=========

Truck Hire is an invoice system for hiring trucks to travel across locations in Australia.  
Ideal application for furniture removalists and and other types of freight transport

How it works:
-------------
Users fill out a form with their origin address, destination address, billing details and number of trucks they wish to hire.  
When the form is completed the user submits, the order is processed according to business logic and a PDF invoice is returned in your web browser.

Features:
--------

Membership system in MS SQL (for applying discounts, benefits etc)  
Google maps api (for distances and travel time)  
iTextSharp PDF Generator (for generating html templated invoices)  
ActiveMQ Publish/Subscriber Message Queue

Requirements:
------------

MS SQL Database  
Internet Connectivity for google maps functionality  
iTextSharp.dll (from sourceforge) for PDF generation  
Apache ActiveMQ for windows
