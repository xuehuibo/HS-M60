﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:2.0.50727.5472
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 此源代码是由 Microsoft.CompactFramework.Design.Data 2.0.50727.5472 版自动生成。
// 
namespace Comm.WRViewSale {
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="ViewSaleSoap", Namespace="http://tempuri.org/")]
    public partial class ViewSale : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        /// <remarks/>
        public ViewSale() {
            this.Url = "http://172.16.134.126/ViewSale.asmx";
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ScanSale", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ScanSale(string OrgCode, string SaleNo, string UserCode, string PassWord) {
            object[] results = this.Invoke("ScanSale", new object[] {
                        OrgCode,
                        SaleNo,
                        UserCode,
                        PassWord});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginScanSale(string OrgCode, string SaleNo, string UserCode, string PassWord, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("ScanSale", new object[] {
                        OrgCode,
                        SaleNo,
                        UserCode,
                        PassWord}, callback, asyncState);
        }
        
        /// <remarks/>
        public string EndScanSale(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
    }
}
