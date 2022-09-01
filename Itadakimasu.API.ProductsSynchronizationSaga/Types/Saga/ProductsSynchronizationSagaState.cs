namespace Itadakimasu.API.ProductsSynchronizationSaga.Types.Saga
{
    using System;

    using JetBrains.Annotations;

    [UsedImplicitly]
    public sealed record ProductsSynchronizationSagaState : MassTransit.SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        
        public ulong RestaurantId { get; set; }

        public string? CurrentState { get; set; } = null!;
        public Guid? RequestId { get; set; }

        public Uri? ResponseAddress { get; set; } = null!;
    }
}