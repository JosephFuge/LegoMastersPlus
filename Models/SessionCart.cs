using System.Text.Json.Serialization;
using LegoMastersPlus.Infrastructure;

namespace LegoMastersPlus.Models;

public class SessionCart : Cart
{
    public static Cart GetCart(IServiceProvider services)
    {
        ISession? session = services.GetRequiredService<IHttpContextAccessor>()
            .HttpContext?.Session;

        SessionCart cart = session?.GetJson<SessionCart>("Cart") ??
                           new SessionCart();

        cart.Session = session;

        return cart;
    }
    
    [JsonIgnore]
    public ISession? Session { get; set; }
    
    public override void AddItem(Project proj, int quantity)
    {
        base.AddItem(proj, quantity);
        Session?.SetJson("Cart", this);
    }

    public override void RemoveLine(Project proj)
    {
        base.RemoveLine(proj);
        Session?.SetJson("Cart", this);
    }

    public override void Clear()
    {
        base.Clear();
        Session?.Remove("Cart");
    }
}