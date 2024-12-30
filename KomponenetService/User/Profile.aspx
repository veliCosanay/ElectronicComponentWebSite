<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="KomponenetService.User.Profile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid pt-5">
        <div class="row px-xl-5">
            <!-- User Profile Section -->
            <div class="col-lg-12 mb-5">
                <div class="card border-secondary mb-5">
                    <div class="card-header bg-secondary border-0">
                        <h4 class="font-weight-semi-bold m-0">My Profile</h4>
                    </div>
                    <div class="card-body">
                        <div class="mb-3">
                            <asp:Label ID="lblMsg" runat="server" Visible="false"></asp:Label>
                        </div>
                        <div class="row">
                            <div class="col-md-4 text-center mb-4">
                                <asp:Image ID="imgProfile" runat="server" CssClass="img-fluid rounded-circle" Width="150" Height="150" />
                                <div class="mt-2">
                                    <asp:FileUpload ID="fuProfilePicture" runat="server" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="col-md-8">
                                <div class="form-group">
                                    <label class="mb-1">Full Name</label>
                                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Enter full name"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ErrorMessage="Name is required"
                                        ControlToValidate="txtName" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                        ValidationGroup="profile">
                                    </asp:RequiredFieldValidator>
                                </div>

                                <div class="form-group">
                                    <label class="mb-1">Email</label>
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter email" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <label class="mb-1">Mobile Number</label>
                                    <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" placeholder="Enter mobile number"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvMobile" runat="server" ErrorMessage="Mobile number is required"
                                        ControlToValidate="txtMobile" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                        ValidationGroup="profile">
                                    </asp:RequiredFieldValidator>
                                </div>

                                <div class="form-group">
                                    <label class="mb-1">Address</label>
                                    <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="Enter address" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ErrorMessage="Address is required"
                                        ControlToValidate="txtAddress" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                        ValidationGroup="profile">
                                    </asp:RequiredFieldValidator>
                                </div>

                                <div class="form-group">
                                    <label class="mb-1">New Password (Leave empty to keep current)</label>
                                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Enter new password" TextMode="Password"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="card-footer border-secondary bg-transparent">
                        <asp:Button ID="btnUpdate" runat="server" Text="Update Profile" CssClass="btn btn-lg btn-block btn-primary font-weight-bold my-3 py-3"
                            OnClick="btnUpdate_Click" ValidationGroup="profile" />
                    </div>
                </div>
            </div>

            <!-- Order History Section -->
            <div class="col-lg-12">
                <div class="card border-secondary mb-5">
                    <div class="card-header bg-secondary border-0">
                        <h4 class="font-weight-semi-bold m-0">Order History</h4>
                    </div>
                    <div class="card-body">
                        <asp:Repeater ID="rOrders" runat="server">
                            <HeaderTemplate>
                                <div class="table-responsive">
                                    <table class="table table-bordered">
                                        <thead class="bg-secondary text-dark">
                                            <tr>
                                                <th>Order Date</th>
                                                <th>Order Details</th>
                                                <th>Amount</th>
                                                <th>Payment Mode</th>
                                                <th>Status</th>
                                                <th>Shipping Address</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%# Convert.ToDateTime(Eval("createddate")).ToString("dd/MM/yyyy HH:mm") %></td>
                                    <td><%# Eval("orderdetails") %></td>
                                    <td>$<%# Eval("amount") %></td>
                                    <td><%# Eval("paymentmode") %></td>
                                    <td><%# Eval("paymentstatus") %></td>
                                    <td><%# Eval("address") %></td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                        </tbody>
                                    </table>
                                </div>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content> 