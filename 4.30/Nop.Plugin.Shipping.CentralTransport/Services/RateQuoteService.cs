using Nop.Core.Domain.Shipping;
using System.Xml;
using System.Net;
using System.IO;
using System;
using Nop.Services.Common;
using System.Configuration;
using Nop.Core;

namespace Nop.Plugin.Shipping.CentralTransport.Services
{
    public class RateQuoteService : IRateQuoteService
    {
        private const string ApiUrl =
            "http://webservices.goctii.com/ratequote.asmx";

        private readonly CentralTransportSettings _settings;
        private readonly ShippingSettings _shippingSettings;
        private readonly IAddressService _addressService;

        public RateQuoteService(
            CentralTransportSettings settings,
            ShippingSettings shippingSettings,
            IAddressService addressService
        )
        {
            _settings = settings;
            _shippingSettings = shippingSettings;
            _addressService = addressService;
        }

        public decimal GetRateQuote(string destZip, int weightInLbs)
        {
            ValidateSettings();

            var shippingOrigin = _addressService.GetAddressById(
                _shippingSettings.ShippingOriginAddressId
            );
            var originZip = shippingOrigin?.ZipPostalCode;
            if (string.IsNullOrWhiteSpace(originZip))
            {
                throw new NopException(
                    "Shipping origin zip/postal " +
                    "code is not set up in Shipping settings, please add the " +
                    "origin zip/postal code");
            }

            XmlDocument soapEnvelopeXml = CreateSoapEnvelope(
                originZip, destZip, weightInLbs
            );
            HttpWebRequest webRequest = CreateWebRequest(ApiUrl);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            // begins async call
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // wait until completion
            asyncResult.AsyncWaitHandle.WaitOne();

            // get the response
            HttpWebResponse webResponse;

            try
            {
                webResponse = (HttpWebResponse)webRequest.EndGetResponse(asyncResult);
            }
            catch (WebException ex)
            {
                webResponse = (HttpWebResponse)ex.Response;

                switch (webResponse.StatusCode)
                {
                    case HttpStatusCode.InternalServerError:
                        var faultString = GetTagValue(webResponse, "faultstring");
                        throw new NopException(
                            "500 error occurred when calling Central " +
                            $"Transport API: {faultString}",
                            ex);

                    default:
                        throw;
                }
            }

            return decimal.Parse(GetTagValue(webResponse, "RateTotal"));
        }

        private string GetTagValue(HttpWebResponse response, string tagName)
        {
            using (StreamReader rd = new StreamReader(
                response.GetResponseStream()
            ))
            {
                string soapResult = rd.ReadToEnd();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(soapResult);
                return doc.GetElementsByTagName(tagName)[0]?.InnerText;
            }
        }

        private void ValidateSettings()
        {
            if (!_settings.IsValid)
            {
                throw new Exception(
                    "Settings are invalid, please " +
                    "configure the plugin settings");
            }
        }

        private static void InsertSoapEnvelopeIntoWebRequest(
            XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        private static HttpWebRequest CreateWebRequest(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private XmlDocument CreateSoapEnvelope(
            string originZip,
            string destZip,
            int weightInLbs)
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(
            $@"<soap:Envelope
                xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" 
                xmlns:xsi=""http://www.w3.org/1999/XMLSchema-instance"" 
                xmlns:xsd=""http://www.w3.org/1999/XMLSchema"">
                <soap:Body>
                    <GetRateQuote xmlns=""http://ci.goctii.com/services"">
                        <accessCode>{_settings.AccessCode}</accessCode>
                        <originZip>{originZip}</originZip>
                        <destinationZip>{destZip}</destinationZip>
                        <customerNumber>{_settings.CustomerNumber}</customerNumber>
                        <shipmentClass>{_settings.ShipmentClass}</shipmentClass>
                        <weight>{weightInLbs}</weight>
                        <accessorial />
                        <type />
                    </GetRateQuote>
                </soap:Body>
            </soap:Envelope>");
            return soapEnvelopeDocument;
        }
    }
}