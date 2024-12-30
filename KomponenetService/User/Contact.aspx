<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="KomponenetService.User.Contact" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Contact Start -->
<div class="container-fluid pt-5">
    <div class="text-center mb-4">
        <h2 class="section-title px-5"><span class="px-2">Contact</span></h2>
    </div>
    <div class="row px-xl-5">
        <div class="col-lg-7 mb-5">
            <div class="contact-form">
                <div class="mb-4">
                    <asp:Label ID="lblMsg" runat="server" Visible="false"></asp:Label>
                </div>
                <div class="form-group mb-3">
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Your Name"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ErrorMessage="Name is required"
                        ControlToValidate="txtName" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                        ValidationGroup="contact">
                    </asp:RequiredFieldValidator>
                </div>
                <div class="form-group mb-3">
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Your Email"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ErrorMessage="Email is required"
                        ControlToValidate="txtEmail" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                        ValidationGroup="contact">
                    </asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revEmail" runat="server" ErrorMessage="Invalid email format"
                        ControlToValidate="txtEmail" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                        ValidationGroup="contact" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">
                    </asp:RegularExpressionValidator>
                </div>
                <div class="form-group mb-3">
                    <asp:TextBox ID="txtSubject" runat="server" CssClass="form-control" placeholder="Subject"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSubject" runat="server" ErrorMessage="Subject is required"
                        ControlToValidate="txtSubject" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                        ValidationGroup="contact">
                    </asp:RequiredFieldValidator>
                </div>
                <div class="form-group mb-3">
                    <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" placeholder="Message"
                        TextMode="MultiLine" Rows="6"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvMessage" runat="server" ErrorMessage="Message is required"
                        ControlToValidate="txtMessage" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                        ValidationGroup="contact">
                    </asp:RequiredFieldValidator>
                </div>
                <div>
                    <asp:Button ID="btnSubmit" runat="server" Text="Send Message" CssClass="btn btn-primary py-2 px-4"
                        OnClick="btnSubmit_Click" ValidationGroup="contact" />
                </div>
            </div>
        </div>
        <div class="col-lg-5 mb-5">
            <h5 class="font-weight-semi-bold mb-3">Get In Touch</h5>
            <p>Justo sed diam ut sed amet duo amet lorem amet stet sea ipsum, sed duo amet et. Est elitr dolor elitr erat sit sit. Dolor diam et erat clita ipsum justo sed.</p>
            <div class="d-flex flex-column mb-3">
                <h5 class="font-weight-semi-bold mb-3">Store 1</h5>
                <p class="mb-2"><i class="fa fa-map-marker-alt text-primary mr-3"></i>123 Street, New York, USA</p>
                <p class="mb-2"><i class="fa fa-envelope text-primary mr-3"></i>info@example.com</p>
                <p class="mb-2"><i class="fa fa-phone-alt text-primary mr-3"></i>+012 345 67890</p>
            </div>
            <div class="d-flex flex-column">
                <h5 class="font-weight-semi-bold mb-3">Store 2</h5>
                <p class="mb-2"><i class="fa fa-map-marker-alt text-primary mr-3"></i>123 Street, New York, USA</p>
                <p class="mb-2"><i class="fa fa-envelope text-primary mr-3"></i>info@example.com</p>
                <p class="mb-0"><i class="fa fa-phone-alt text-primary mr-3"></i>+012 345 67890</p>
            </div>
        </div>
    </div>
</div>
<!-- Contact End -->

</asp:Content>
