﻿namespace Uno.Extensions.Navigation.Navigators;

public abstract class DialogNavigator : ControlNavigator
{
	public override bool CanGoBack => true;

	private IAsyncInfo? ShowTask { get; set; }

	protected DialogNavigator(
		ILogger<DialogNavigator> logger,
		IWindowProvider window,
		IRegion region,
		IResolver resolver)
		: base(logger, window, region, resolver)
	{
	}

	protected override async Task<Route?> ExecuteRequestAsync(NavigationRequest request)
	{
		var route = request.Route;
		// If this is back navigation, then make sure it's used to close
		// any of the open dialogs
		if (route.FrameIsBackNavigation() && ShowTask is not null)
		{
			await CloseDialog();
		}
		else
		{
			var mapping = Resolver.Routes.Find(route);
			var viewModel = (Region.Services is not null && mapping?.View?.ViewModel is not null) ? await CreateViewModel(Region.Services, request, route, mapping) : default(object);
			ShowTask = await DisplayDialog(request, mapping?.View?.RenderView, viewModel);
		}
		var responseRequest = route with { Path = null };
		return responseRequest;
	}

	protected async Task CloseDialog()
	{
		var dialog = ShowTask;
		ShowTask = null;

		dialog?.Cancel();
	}

	protected abstract Task<IAsyncInfo?> DisplayDialog(NavigationRequest request, Type? viewType, object? viewModel);
}
