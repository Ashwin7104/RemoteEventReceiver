using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace RERAshwinWeb.Services
{
    public class AppEventReceiver : IRemoteEventService
    {

        private const string ReceiverName = "ItemAddedEvent";
        private const string ListName = "RERList";

        /// <summary>
        /// Handles app events that occur after the app is installed or upgraded, or when app is being uninstalled.
        /// </summary>
        /// <param name="properties">Holds information about the app event.</param>
        /// <returns>Holds information returned from the app event.</returns>
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            /*  SPRemoteEventResult result = new SPRemoteEventResult();

              using (ClientContext clientContext = TokenHelper.CreateAppEventClientContext(properties, useAppWeb: false))
              {
                  if (clientContext != null)
                  {
                      clientContext.Load(clientContext.Web);
                      clientContext.ExecuteQuery();
                  }
              }

              return result;*/
            SPRemoteEventResult result = new SPRemoteEventResult();
            switch (properties.EventType)
            {
                case SPRemoteEventType.AppInstalled:
                    HandleAppInstalled(properties);
                    break;
                case SPRemoteEventType.AppUninstalling:
                    //  HandleAppUninstalling(properties);
                    break;
                case SPRemoteEventType.ItemAdded:
                    HandleItemAdded(properties);
                    break;
            }
            return result;
        }

        /// <summary>
        /// This method is a required placeholder, but is not used by app events.
        /// </summary>
        /// <param name="properties">Unused.</param>
        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method runs during the App Installion process and used to create a new list and attach a itemAdded event to it.
        /// </summary>
        /// <param name="properties"></param>
        private void HandleAppInstalled(SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext = TokenHelper.CreateAppEventClientContext(properties, useAppWeb: false))
            {
                if (clientContext != null)
                {

                    ListCreationInformation creationInfo = new ListCreationInformation();
                    creationInfo.Title = ListName;
                    creationInfo.Description = "New list created through Remote receiver";

                    // using Generic List Template
                    creationInfo.TemplateType = (int)ListTemplateType.GenericList;
                    // Create a new custom list    

                    List myList = clientContext.Web.Lists.Add(creationInfo);
                    // Retrieve the custom list properties    
                    clientContext.Load(myList);
                    // Execute the query to the server.    
                    clientContext.ExecuteQuery();

                    // List myList = clientContext.Web.Lists.GetByTitle(ListName);
                    clientContext.Load(myList, p => p.EventReceivers);
                    clientContext.ExecuteQuery();

                    bool rerExists = false;

                    foreach (var rer in myList.EventReceivers)
                    {
                        if (rer.ReceiverName == ReceiverName)
                        {
                            rerExists = true;
                            System.Diagnostics.Trace.WriteLine("Found existing ItemAdded receiver at "
                                + rer.ReceiverUrl);
                        }
                    }

                    if (!rerExists)
                    {
                        EventReceiverDefinitionCreationInformation receiver =
                            new EventReceiverDefinitionCreationInformation();
                        receiver.EventType = EventReceiverType.ItemAdded;

                        //Get WCF URL where this message was handled
                        OperationContext op = OperationContext.Current;
                        Message msg = op.RequestContext.RequestMessage;

                        receiver.ReceiverUrl = msg.Headers.To.ToString();

                        receiver.ReceiverName = ReceiverName;
                        receiver.Synchronization = EventReceiverSynchronization.Synchronous;
                        myList.EventReceivers.Add(receiver);

                        clientContext.ExecuteQuery();

                        System.Diagnostics.Trace.WriteLine("Added ItemAdded receiver at "
                                + msg.Headers.To.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// This method gets triggered when an item is added to the newly created list.
        /// </summary>
        /// <param name="properties"></param>
        private void HandleItemAdded(SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext = TokenHelper.CreateRemoteEventReceiverClientContext(properties))
            {
                if (clientContext != null)
                {
                    try
                    {
                        List lstReceiver = clientContext.Web.Lists.GetById(properties.ItemEventProperties.ListId);
                        ListItem item = lstReceiver.GetItemById(properties.ItemEventProperties.ListItemId);
                        clientContext.Load(item);
                        clientContext.ExecuteQuery();

                        item["Title"] += " - updated through RER on  " + System.DateTime.Now.ToLongTimeString();
                        item.Update();
                        clientContext.ExecuteQuery();

                        System.Diagnostics.Trace.WriteLine("Item Added - Updated item through remote event receiver");
                    }
                    catch (Exception oops)
                    {
                        System.Diagnostics.Trace.WriteLine(oops.Message);
                    }
                }

            }
        }
    }
}
