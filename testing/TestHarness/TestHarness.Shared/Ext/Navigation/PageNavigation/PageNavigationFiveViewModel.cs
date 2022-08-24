﻿namespace TestHarness.Ext.Navigation.PageNavigation;

public record PageNavigationFiveViewModel(INavigator Navigator, IWritableOptions<PageNavigationSettings> Settings)
{
	public async void GoBack()
	{
		await Settings.UpdateAsync(s => s with { PagesVisited = s.PagesVisited.Add(this.GetType().Name) });
		await Navigator.GoBack(this);
	}
}
