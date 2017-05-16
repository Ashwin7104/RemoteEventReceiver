The code is used as an sample to create a list and attach item added event receiver on the App Install method (on the fly) in host web.

Please use below reference for more information

1)https://blogs.msdn.microsoft.com/kaevans/2014/02/26/attaching-remote-event-receivers-to-lists-in-the-host-web/
2)https://github.com/SharePoint/PnP-Guidance/blob/master/articles/Use-remote-event-receivers-in-SharePoint.md



Please use below code to generate ACS connection string

New-AzureSBNamespace -Name RERAShwin -Location "Central US" -CreateACSNamespace $true -NamespaceType Messaging 


/*********************Sample *********************************/

Name                  : RERAShwin
Region                : Central US
DefaultKey            : bN8yjBdv8NFJWU8babNH1U7pSHG5Ita0u7tGwhwkHeM=
Status                : Active
CreatedAt             : 16-05-2017 16:30:11
AcsManagementEndpoint : https://rerashwin-sb.accesscontrol.windows.net/
ServiceBusEndpoint    : https://rerashwin.servicebus.windows.net/
ConnectionString      : Endpoint=sb://rerashwin.servicebus.windows.net/;SharedS
                        ecretIssuer=owner;SharedSecretValue=bN8yjBdv8NFJWU8babN
                        H1U7pSHG5Ita0u7tGwhwkHeM=
NamespaceType         : Messaging


