using Microsoft.AspNetCore.Components;

namespace SynapseVue.Server.Components;

[StreamRendering(enabled: true)]
public partial class App
{
    [CascadingParameter] HttpContext HttpContext { get; set; } = default!;
}
