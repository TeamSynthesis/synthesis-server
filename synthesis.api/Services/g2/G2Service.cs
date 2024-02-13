using synthesis.api.Data.Models;

namespace synthesis.api.Services.g2;

public interface IG2Service
{
    public List<Competitor> GetCompetitors(string appDescription);
}

public class G2Service : IG2Service
{
    public List<Competitor> GetCompetitors(string appDescription)
    {
        return new List<Competitor> {

             new Competitor { Name = "NotionAI" ,Description = "notion is an ai powered project manament tool", ReviewSentiment = 0.7 , PricingModel = "Hybrid, Freemium", Size= "Multi-National"},
             new Competitor { Name = "ClickUp" ,Description = " is an  innovative project management tool", ReviewSentiment = 0.8, PricingModel = "Subscription Based", Size = "Large" },
             new Competitor { Name = "SynthesisAI" ,Description = " is a  ai driven project management assitant", ReviewSentiment = 0.7, PricingModel = "Freemium", Size = "Medium" }
        };
    }
}