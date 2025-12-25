using Microsoft.AspNetCore.Components;
using ScrumTrainer.BusinessLogic;
using ScrumTrainer.Models;

namespace ScrumTrainer.Components;

public partial class ScoresViewer
{
    [Parameter] public required ResultsProvider ResultsProvider { get; set; }
    public ICollection<QuizResult> QuizResults { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        QuizResults = ResultsProvider is not null 
            ? await ResultsProvider.QuizResults 
            : [];
    }
}
