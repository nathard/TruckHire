<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" Theme="Theme1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script src="https://maps.googleapis.com/maps/api/js?sensor=false&libraries=places" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">
        function ValidateFields() {
            
            // Validate Addresses
            validator = new RegExp("^[a-zA-Z0-9 ]*$");
            if (validator.test(document.getElementById("txtAddress1").value)  == false) 
            {

                alert("Invalid Origin Address");
                return false;
            }

            validator = new RegExp("^[a-zA-Z0-9 ]*$");
            if (validator.test(document.getElementById("txtAddress2").value) == false) {

                alert("Invalid Destination Address");
                return false;
            }

            validator = new RegExp("^[a-zA-Z0-9 ]*$");
            if (validator.test(document.getElementById("txtAddress3").value) == false) {

                alert("Invalid Billing Address");
                return false;
            }

            // Validate Full Names
            validator = new RegExp("^[a-zA-Z ]*$");
            if (validator.test(document.getElementById("txtFullName1").value) == false) {

                alert("Invalid Full Name (Origin)");
                return false;
            }

            validator = new RegExp("^[a-zA-Z ]*$");
            if (validator.test(document.getElementById("txtFullName1").value) == false) {

                alert("Invalid Full Name (Destination)");
                return false;
            }

            validator = new RegExp("^[a-zA-Z ]*$");
            if (validator.test(document.getElementById("txtFullName2").value) == false) {

                alert("Invalid Full Name (Billing)");
                return false;
            }

            // Validate Town/Cities
            validator = new RegExp("^[a-zA-Z ]*$");
            if (validator.test(document.getElementById("txtCity1").value) == false) {

                alert("Invalid City (Origin)");
                return false;
            }

            validator = new RegExp("^[a-zA-Z ]*$");
            if (validator.test(document.getElementById("txtCity2").value) == false) {

                alert("Invalid City (Destination)");
                return false;
            }

            validator = new RegExp("^[a-zA-Z ]*$");
            if (validator.test(document.getElementById("txtCity2").value) == false) {

                alert("Invalid City (Billing)");
                return false;
            }

            // Validate Post Codes
            var validator = new RegExp("^((0[289][0-9]{2})|([1345689][0-9]{3})|(2[0-8][0-9]{2})|(290[0-9])|(291[0-4])|(7[0-4][0-9]{2})|(7[8-9][0-9]{2}))*$");
            if (validator.test(document.getElementById("txtPostCode1").value) == false) {

                alert("Invalid Post Code (Origin)");
                return false;
            }

            var validator = new RegExp("^((0[289][0-9]{2})|([1345689][0-9]{3})|(2[0-8][0-9]{2})|(290[0-9])|(291[0-4])|(7[0-4][0-9]{2})|(7[8-9][0-9]{2}))*$");
            if (validator.test(document.getElementById("txtPostCode2").value) == false) {

                alert("Invalid Post Code (Destination)");
                return false;
            }

            var validator = new RegExp("^((0[289][0-9]{2})|([1345689][0-9]{3})|(2[0-8][0-9]{2})|(290[0-9])|(291[0-4])|(7[0-4][0-9]{2})|(7[8-9][0-9]{2}))*$");
            if (validator.test(document.getElementById("txtPostCode3").value) == false) {

                alert("Invalid Post Code (Billing)");
                return false;
            }

            // Validate Phone
            var validator = new RegExp("^[0-9]+$");
            if (validator.test(document.getElementById("txtPhone").value) == false) {

                alert("Invalid Phone number");
                return false;
            }

            // Validate Trucks
            var validator = new RegExp("^[0-9]$");
            if (validator.test(document.getElementById("txtTrucks").value) == false) {

                alert("Invalid Number of Trucks");
                return false;
            }
        }
    </script>
</head>
<body>
<div id="wrap">
    <form id="form1" runat="server">
    <div id="header">   
        <h1>Truck Hire</h1>
    </div>
    <br />
    <div id="main">
    <!-- Membership controls -->
        <asp:Label ID="Label1" Text="Enter your Membership Id: " runat="server" />
        <br />
        <asp:TextBox ID="txtMemberId" runat="server" />
        <asp:Button ID="Button1" Text="Search" OnClick="btnMember" runat="server" ValidationGroup="MemVal" />
        <asp:RequiredFieldValidator ID="rqdMemId" ControlToValidate="txtMemberId" ValidationGroup="MemVal" Text="(Required)" runat="server" />
        <asp:CompareValidator ID="cmpMemId" ControlToValidate="txtMemberId" ValidationGroup="MemVal" Text="(Must be a number)" Operator="DataTypeCheck" Type="Integer" runat="server" />
        <br />
        <asp:Label ID="lblMember" runat="server" />
        
        <hr /><br />
    <!-- Truck Hire controls -->
        <div id="origin">
            <b>Origin</b>
            <br />
            <asp:Label ID="lblFullName1" Text="Full Name:" AssociatedControlID="txtFullName1" runat="server" />
            <asp:TextBox ID="txtFullName1" runat="server" />
            <asp:RequiredFieldValidator ID="reqFullName1" ControlToValidate="txtFullName1" Text="(Required)" runat="server" />
            <br />

            <asp:Label ID="lblAddress1" Text="Address:" AssociatedControlID="txtAddress1" runat="server" />
            <asp:TextBox ID="txtAddress1" runat="server" />
            <asp:RequiredFieldValidator ID="reqAddress1" ControlToValidate="txtAddress1" Text="(Required)" runat="server" />
            <br />

            <asp:Label ID="lblCity1" Text="Town/City:" AssociatedControlID="txtCity1" runat="server" />
            <asp:TextBox ID="txtCity1" runat="server" />
            <asp:RequiredFieldValidator ID="reqCity1" ControlToValidate="txtCity1" Text="(Required)" runat="server" />
            <br />

            <asp:Label ID="lblState1" Text="State:" AssociatedControlID="txtState1" runat="server" />
            <asp:DropDownList ID="txtState1" runat="server">
            <asp:ListItem></asp:ListItem>
            <asp:ListItem>ACT</asp:ListItem>
            <asp:ListItem>NSW</asp:ListItem>
            <asp:ListItem>NT</asp:ListItem>
            <asp:ListItem>QLD</asp:ListItem>
            <asp:ListItem>SA</asp:ListItem>
            <asp:ListItem>TAS</asp:ListItem>
            <asp:ListItem>VIC</asp:ListItem>
            <asp:ListItem>WA</asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="reqState1" ControlToValidate="txtState1" Text="(Required)" runat="server" />
            <br />

            <asp:Label ID="lblPostCode1" Text="Post Code:" AssociatedControlID="txtPostCode1" runat="server" />
            <asp:TextBox ID="txtPostCode1" Columns="4" runat="server" />
            <asp:RequiredFieldValidator ID="reqPostCode1" ControlToValidate="txtPostCode1" Text="(Required)" runat="server" />
            
            <br />

        </div>
        <div id="destination">
            <b>Destination</b>
            <br />
            <asp:Label ID="lblFullName2" Text="Full Name:" AssociatedControlID="txtFullName2" runat="server" />
            <asp:TextBox ID="txtFullName2" runat="server" />
            <asp:RequiredFieldValidator ID="reqFullName2" ControlToValidate="txtFullName2" Text="(Required)" runat="server" />
            <br />

            <asp:Label ID="lblAddress2" Text="Address:" AssociatedControlID="txtAddress2" runat="server" />
            <asp:TextBox ID="txtAddress2" runat="server" />
            <asp:RequiredFieldValidator ID="reqAddress2" ControlToValidate="txtAddress2" Text="(Required)" runat="server" />
            <br />

            <asp:Label ID="lblCity2" Text="Town/City:" AssociatedControlID="txtCity2" runat="server" />
            <asp:TextBox ID="txtCity2" runat="server" />
            <asp:RequiredFieldValidator ID="reqCity2" ControlToValidate="txtCity2" Text="(Required)" runat="server" />
            <br />

            <asp:Label ID="lblState2" Text="State:" AssociatedControlID="txtState2" runat="server" />
            <asp:DropDownList ID="txtState2" runat="server">
            <asp:ListItem></asp:ListItem>
            <asp:ListItem>ACT</asp:ListItem>
            <asp:ListItem>NSW</asp:ListItem>
            <asp:ListItem>NT</asp:ListItem>
            <asp:ListItem>QLD</asp:ListItem>
            <asp:ListItem>SA</asp:ListItem>
            <asp:ListItem>TAS</asp:ListItem>
            <asp:ListItem>VIC</asp:ListItem>
            <asp:ListItem>WA</asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="reqState2" ControlToValidate="txtState2" Text="(Required)" runat="server" />
            <br />

            <asp:Label ID="lblPostCode2" Text="Post Code:" AssociatedControlID="txtPostCode2" runat="server" />
            <asp:TextBox ID="txtPostCode2" Columns="4" runat="server" />
            <asp:RequiredFieldValidator ID="reqPostCode2" ControlToValidate="txtPostCode2" Text="(Required)" runat="server" />
            <br />

        </div>
        <br /><br />
        <div id="billing">
            <br />
            <b>Billing:</b>
            <br />
            <asp:Label ID="lblFullName3" Text="Full Name:" AssociatedControlID="txtFullName3" runat="server" />
            <asp:TextBox ID="txtFullName3" runat="server" />
            <asp:RequiredFieldValidator ID="req" ControlToValidate="txtFullName3" Text="(Required)" runat="server" />
            <br />

            <asp:Label ID="lblPhone" Text="Phone Number:" AssociatedControlID="txtPhone" runat="server" />
            <asp:TextBox ID="txtPhone" runat="server" />
            <asp:RequiredFieldValidator ID="reqPhone" ControlToValidate="txtPhone" Text="(Required)" runat="server" />
            <br />

            <asp:Label ID="lblAddress3" Text="Address:" AssociatedControlID="txtAddress3" runat="server" />
            <asp:TextBox ID="txtAddress3" runat="server" />
            <asp:RequiredFieldValidator ID="reqAddress3" ControlToValidate="txtAddress3" Text="(Required)" runat="server" />
            <br />

            <asp:Label ID="lblCity3" Text="Town/City:" AssociatedControlID="txtCity3" runat="server" />
            <asp:TextBox ID="txtCity3" runat="server" />
            <asp:RequiredFieldValidator ID="reqCity3" ControlToValidate="txtCity3" Text="(Required)" runat="server" />
            <br />

            <asp:Label ID="lblState3" Text="State:" AssociatedControlID="txtState3" runat="server" />
            <asp:DropDownList ID="txtState3" runat="server">
            <asp:ListItem></asp:ListItem>
            <asp:ListItem>ACT</asp:ListItem>
            <asp:ListItem>NSW</asp:ListItem>
            <asp:ListItem>NT</asp:ListItem>
            <asp:ListItem>QLD</asp:ListItem>
            <asp:ListItem>SA</asp:ListItem>
            <asp:ListItem>TAS</asp:ListItem>
            <asp:ListItem>VIC</asp:ListItem>
            <asp:ListItem>WA</asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="reqState3" ControlToValidate="txtState3" Text="(Required)" runat="server" />
            <br />

            <asp:Label ID="lblPostCode3" Text="Post Code:" AssociatedControlID="txtPostCode3" runat="server" />
            <asp:TextBox ID="txtPostCode3" Columns="4" runat="server" />
            <asp:RequiredFieldValidator ID="reqPostCode3" ControlToValidate="txtPostCode3" Text="(Required)" runat="server" />
            <br />

        </div>
        <br />
        <div id="order">
            <br />
            <b>Order</b>
            <br />
            <asp:Label ID="lblTrucks" Text="Number Of Trucks:" AssociatedControlID="txtTrucks" runat="server" />
            <asp:TextBox ID="txtTrucks" Columns="2" runat="server" />
            <asp:RequiredFieldValidator ID="reqTrucks" ControlToValidate="txtTrucks" Text="(Required)" runat="server" />
            <br />

            <asp:Label ID="lblDate" Text="Delivery Date:" AssociatedControlID="txtDate" runat="server" />
            <asp:TextBox ID="txtDate" Columns="5" runat="server" />
            <asp:RequiredFieldValidator ID="reqDate" ControlToValidate="txtDate" Text="(Required)" runat="server" />
            <asp:CompareValidator ID="cmpDate" ControlToValidate="txtDate" Text="(Invalid Date e.g. 31-12-2020)" Operator="DataTypeCheck" Type="Date" runat="server" />
            <br />

            <asp:Label ID="lblTime" Text="Delivery Time:" AssociatedControlID="txtTime" runat="server" />
            <asp:DropDownList ID="txtTime" runat="server">
            <asp:ListItem></asp:ListItem>
            <asp:ListItem>1:00 AM</asp:ListItem>
            <asp:ListItem>2:00 AM</asp:ListItem>
            <asp:ListItem>3:00 AM</asp:ListItem>
            <asp:ListItem>4:00 AM</asp:ListItem>
            <asp:ListItem>5:00 AM</asp:ListItem>
            <asp:ListItem>6:00 AM</asp:ListItem>
            <asp:ListItem>7:00 AM</asp:ListItem>
            <asp:ListItem>8:00 AM</asp:ListItem>
            <asp:ListItem>9:00 AM</asp:ListItem>
            <asp:ListItem>10:00 AM</asp:ListItem>
            <asp:ListItem>11:00 AM</asp:ListItem>
            <asp:ListItem>12:00 AM</asp:ListItem>
            <asp:ListItem>1:00 PM</asp:ListItem>
            <asp:ListItem>2:00 PM</asp:ListItem>
            <asp:ListItem>3:00 PM</asp:ListItem>
            <asp:ListItem>4:00 PM</asp:ListItem>
            <asp:ListItem>5:00 PM</asp:ListItem>
            <asp:ListItem>6:00 PM</asp:ListItem>
            <asp:ListItem>7:00 PM</asp:ListItem>
            <asp:ListItem>8:00 PM</asp:ListItem>
            <asp:ListItem>9:00 PM</asp:ListItem>
            <asp:ListItem>10:00 PM</asp:ListItem>
            <asp:ListItem>11:00 PM</asp:ListItem>
            <asp:ListItem>12:00 PM</asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="reqTime" ControlToValidate="txtTime" Text="(Required)" runat="server" />
            
            <br />
            <br />
            <asp:Button ID="btnValidate" runat="server" Text="Submit Order" OnClick="buttonValidate" OnClientClick="if (ValidateFields() == false) return(false);" />

            <br />
            <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
            <br />
            <asp:Label ID="lblError" runat="server" />
        </div>
    </div>
    </form>
</div>
</body>
</html>
