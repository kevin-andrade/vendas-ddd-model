using FluentAssertions;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Orders;
using Vendas.Domain.Orders.Enums;
using Vendas.Domain.Orders.Events;

namespace Vendas.Domain.Tests.Orders.Entities;

public class PaymentTests
{
      [Fact(DisplayName = "Deve criar um pagamento válido com status Pending")]
    public void Deve_Criar_Pagamento_Valido_Com_Status_Pending()
    {
        var orderId = Guid.NewGuid();
        var metodo = PaymentMethod.CreditCard;
        var amount = 100m;

        var pagamento = new Payment(orderId, metodo, amount);

        pagamento.OrderId.Should().Be(orderId);
        pagamento.PaymentMethod.Should().Be(metodo);
        pagamento.Amount.Should().Be(amount);
        pagamento.PaymentStatus.Should().Be(PaymentStatus.Pending);
        pagamento.PaidAt.Should().BeNull();
        pagamento.TransactionCode.Should().BeNull();
    }

    [Fact(DisplayName = "Não deve criar pagamento com Amount menor ou igual a zero")]
    public void Nao_Deve_Criar_Pagamento_Com_Amount_Invalido()
    {
        var OrderId = Guid.NewGuid();

        Action act = () => new Payment(OrderId, PaymentMethod.Pix, 0);

        act.Should()
           .Throw<DomainException>()
           .WithMessage("O Amount do pagamento deve ser maior que zero.");
    }

    [Fact(DisplayName = "Não deve definir código de transação nulo ou vazio")]
    public void Nao_Deve_Definir_Codigo_Transacao_Nulo_Ou_Vazio()
    {
        var pagamento = new Payment(Guid.NewGuid(), PaymentMethod.Pix, 100m);

        Action act = () => pagamento.SetTransactionCode("");

        act.Should()
           .Throw<DomainException>()
           .WithMessage("Código da transação inválido.");
    }

    [Fact(DisplayName = "Deve definir código de transação válido e atualizar DataAtualizacao")]
    public void Deve_Definir_Codigo_Transacao_Valido()
    {
        var pagamento = new Payment(Guid.NewGuid(), PaymentMethod.CreditCard, 100m);
        var codigo = "TXN-12345";

        pagamento.SetTransactionCode(codigo);

        pagamento.TransactionCode.Should().Be(codigo);
        pagamento.UpdatedAt.Should().NotBeNull();
    }

    [Fact(DisplayName = "Não deve redefinir código de transação já definido")]
    public void Nao_Deve_Redefinir_Codigo_Transacao()
    {
        var pagamento = new Payment(Guid.NewGuid(), PaymentMethod.CreditCard, 100m);
        pagamento.SetTransactionCode("TXN-001");

        Action act = () => pagamento.SetTransactionCode("TXN-002");

        act.Should()
           .Throw<DomainException>()
           .WithMessage("O código de transação já foi definido.");
    }

    [Fact(DisplayName = "Deve gerar código de transação local automaticamente")]
    public void Deve_Gerar_Codigo_Transacao_Local()
    {
        var pagamento = new Payment(Guid.NewGuid(), PaymentMethod.Pix, 200m);

        pagamento.GenerateTransactionCodeLocal();

        pagamento.TransactionCode.Should().StartWith("LOCAL-");
        pagamento.TransactionCode.Should().HaveLength(14); // LOCAL- + 8 chars
        pagamento.UpdatedAt.Should().NotBeNull();
    }

    [Fact(DisplayName = "Deve confirmar pagamento Pending com código válido e gerar evento completo")]
    public void Deve_Confirmar_Pagamento_Com_Codigo_Valido_E_Evento_Completo()
    {
        var pagamento = new Payment(Guid.NewGuid(), PaymentMethod.CreditCard, 300m);
        pagamento.GenerateTransactionCodeLocal(); // Simula gateway

        pagamento.ConfirmPayment();

        pagamento.PaymentStatus.Should().Be(PaymentStatus.Approved);
        pagamento.PaidAt.Should().NotBeNull();
        pagamento.UpdatedAt.Should().NotBeNull();

        var evento = pagamento.DomainEvents.OfType<PaymentApprovedEvent>().FirstOrDefault();
        evento.Should().NotBeNull();
        evento!.PaymentId.Should().Be(pagamento.Id);
        evento.OrderId.Should().Be(pagamento.OrderId);
        evento.Amount.Should().Be(pagamento.Amount);
        evento.TransactionCode.Should().Be(pagamento.TransactionCode);
        evento.PaidAt.Should().Be(pagamento.PaidAt);
    }

    [Fact(DisplayName = "Não deve confirmar pagamento sem código de transação")]
    public void Nao_Deve_Confirmar_Sem_Codigo_Transacao()
    {
        var pagamento = new Payment(Guid.NewGuid(), PaymentMethod.Pix, 100m);

        Action act = () => pagamento.ConfirmPayment();

        act.Should()
           .Throw<DomainException>()
           .WithMessage("O pagamento não pode ser confirmado sem o código de transação.");
    }
    [Fact(DisplayName = "Não deve confirmar pagamento que não está Pending")]
    public void Nao_Deve_Confirmar_Pagamento_Que_Nao_Esta_Pending()
    {
        var pagamento = new Payment(Guid.NewGuid(), PaymentMethod.Pix, 100m);
        pagamento.GenerateTransactionCodeLocal();
        pagamento.ConfirmPayment();

        Action act = () => pagamento.ConfirmPayment();

        act.Should()
           .Throw<DomainException>()
           .WithMessage("Apenas pagamentos Pendings podem ser confirmados.");
    }

    [Fact(DisplayName = "Deve recusar pagamento Pending e gerar evento de rejeição com dados corretos")]
    public void Deve_Recusar_Pagamento_Pending_E_Gerar_Evento_Com_Dados()
    {
        var pagamento = new Payment(Guid.NewGuid(), PaymentMethod.Pix, 120m);

        pagamento.RecusarPagamento();

        pagamento.PaymentStatus.Should().Be(PaymentStatus.Declined);
        pagamento.PaidAt.Should().NotBeNull();
        pagamento.UpdatedAt.Should().NotBeNull();

        var evento = pagamento.DomainEvents.OfType<PaymentDeclinedEvent>().FirstOrDefault();
        evento.Should().NotBeNull();
        evento!.PaymentId.Should().Be(pagamento.Id);
        evento.OrderId.Should().Be(pagamento.OrderId);
        evento.Amount.Should().Be(pagamento.Amount);
        evento.TransactionCode.Should().Be(pagamento.TransactionCode);
        evento.PaidAt.Should().Be(pagamento.PaidAt);
    }

    [Fact(DisplayName = "Não deve recusar pagamento que não está Pending")]
    public void Nao_Deve_Recusar_Pagamento_Que_Nao_Esta_Pending()
    {
        var pagamento = new Payment(Guid.NewGuid(), PaymentMethod.Pix, 120m);
        pagamento.GenerateTransactionCodeLocal();
        pagamento.ConfirmPayment();

        Action act = () => pagamento.RecusarPagamento();

        act.Should()
           .Throw<DomainException>()
           .WithMessage("Apenas pagamentos Pendings podem ser recusados.");
    }
}