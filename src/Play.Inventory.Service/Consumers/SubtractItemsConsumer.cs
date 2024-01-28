using System;
using System.Threading.Tasks;
using DnsClient.Internal;
using MassTransit;
using Microsoft.Extensions.Logging;
using Play.Common;
using Play.Inventory.Contracts;
using Play.Inventory.Service.Entities;
using Play.Inventory.Service.Exceptions;

namespace Play.Inventory.Service.Consumers
{
    public class SubtractItemsConsumer : IConsumer<SubtractItems>
    {
        private readonly ILogger<SubtractItemsConsumer> logger;
        private readonly IRepository<InventoryItem> inventoryItemsRepository;
        private readonly IRepository<CatalogItem> catalogItemsRepository;

        public SubtractItemsConsumer(IRepository<InventoryItem> inventoryItemsRepository, IRepository<CatalogItem> catalogItemsRepository, ILogger<SubtractItemsConsumer> logger)
        {
            this.inventoryItemsRepository = inventoryItemsRepository;
            this.catalogItemsRepository = catalogItemsRepository;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<SubtractItems> context)
        {
            logger.LogInformation(
                "Subtracting quantity {Quantity} of item {CatalogItemId} from user {UserId} with CorrelationId {CorrelationId}",
                context.Message.Quantity,
                context.Message.CatalogItemId,
                context.Message.UserId,
                context.Message.CorrelationId
            );

            var message = context.Message;

            var item = await catalogItemsRepository.GetAsync(message.CatalogItemId);

            if (item == null)
            {
                throw new UnknownItemException(message.CatalogItemId);
            }

            var inventoryItem = await inventoryItemsRepository.GetAsync(item => item.UserId == message.UserId && item.CatalogItemId == message.CatalogItemId);

            if (inventoryItem != null)
            {
                if (inventoryItem.MessageIds.Contains(context.MessageId.Value))
                {
                    await context.Publish(new InventoryItemsSubtracted(message.CorrelationId));
                    return;
                }

                inventoryItem.Quantity -= message.Quantity;
                inventoryItem.MessageIds.Add(context.MessageId.Value);

                await inventoryItemsRepository.UpdateAsync(inventoryItem);

                await context.Publish(new InventoryItemUpdated(
                    inventoryItem.UserId,
                    inventoryItem.CatalogItemId,
                    inventoryItem.Quantity
                ));
            }

            await context.Publish(new InventoryItemsSubtracted(message.CorrelationId));
        }
    }
}