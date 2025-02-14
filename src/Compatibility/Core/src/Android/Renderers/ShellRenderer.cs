using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using AndroidX.DrawerLayout.Widget;
using AndroidX.Fragment.App;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Graphics;
using AColor = Android.Graphics.Color;
using ARect = Android.Graphics.Rect;
using AView = Android.Views.View;
using Color = Microsoft.Maui.Graphics.Color;
using LP = Android.Views.ViewGroup.LayoutParams;
using Paint = Android.Graphics.Paint;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Microsoft.Maui.Controls.Compatibility.Platform.Android
{
	public class ShellRenderer : IVisualElementRenderer, IShellContext, IAppearanceObserver
	{
		#region IVisualElementRenderer

		event EventHandler<VisualElementChangedEventArgs> IVisualElementRenderer.ElementChanged
		{
			add { _elementChanged += value; }
			remove { _elementChanged -= value; }
		}

		event EventHandler<PropertyChangedEventArgs> IVisualElementRenderer.ElementPropertyChanged
		{
			add { _elementPropertyChanged += value; }
			remove { _elementPropertyChanged -= value; }
		}

		VisualElement IVisualElementRenderer.Element => Element;

		VisualElementTracker IVisualElementRenderer.Tracker => null;

		AView IVisualElementRenderer.View => _flyoutRenderer.AndroidView;

		SizeRequest IVisualElementRenderer.GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			return new SizeRequest(new Size(100, 100));
		}

		void IVisualElementRenderer.SetElement(VisualElement element)
		{
			if (Element != null)
				throw new NotSupportedException("Reuse of the Shell Renderer is not supported");
			Element = (Shell)element;
			Element.SizeChanged += OnElementSizeChanged;
			OnElementSet(Element);

			Element.PropertyChanged += OnElementPropertyChanged;
			_elementChanged?.Invoke(this, new VisualElementChangedEventArgs(null, Element));
		}

		void IVisualElementRenderer.SetLabelFor(int? id)
		{
		}

		// Used by Previewer
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void UpdateLayout()
		{
			var width = (int)AndroidContext.ToPixels(Element.Width);
			var height = (int)AndroidContext.ToPixels(Element.Height);
			_flyoutRenderer.AndroidView.Layout(0, 0, width, height);
		}

		#endregion IVisualElementRenderer

		#region IShellContext

		Context IShellContext.AndroidContext => AndroidContext;

		// This is very bad, FIXME.
		// This assumes all flyouts will implement via DrawerLayout which is PROBABLY true but
		// I dont want to back us into a corner this time.
		DrawerLayout IShellContext.CurrentDrawerLayout => (DrawerLayout)_flyoutRenderer.AndroidView;

		Shell IShellContext.Shell => Element;

		IShellObservableFragment IShellContext.CreateFragmentForPage(Page page)
		{
			return CreateFragmentForPage(page);
		}

		IShellFlyoutContentRenderer IShellContext.CreateShellFlyoutContentRenderer()
		{
			return CreateShellFlyoutContentRenderer();
		}

		IShellItemRenderer IShellContext.CreateShellItemRenderer(ShellItem shellItem)
		{
			return CreateShellItemRenderer(shellItem);
		}

		IShellSectionRenderer IShellContext.CreateShellSectionRenderer(ShellSection shellSection)
		{
			return CreateShellSectionRenderer(shellSection);
		}

		IShellToolbarTracker IShellContext.CreateTrackerForToolbar(Toolbar toolbar)
		{
			return CreateTrackerForToolbar(toolbar);
		}

		IShellToolbarAppearanceTracker IShellContext.CreateToolbarAppearanceTracker()
		{
			return CreateToolbarAppearanceTracker();
		}

		IShellTabLayoutAppearanceTracker IShellContext.CreateTabLayoutAppearanceTracker(ShellSection shellSection)
		{
			return CreateTabLayoutAppearanceTracker(shellSection);
		}

		IShellBottomNavViewAppearanceTracker IShellContext.CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
		{
			return CreateBottomNavViewAppearanceTracker(shellItem);
		}

		#endregion IShellContext

		#region IAppearanceObserver

		void IAppearanceObserver.OnAppearanceChanged(ShellAppearance appearance)
		{
			UpdateStatusBarColor(appearance);
		}

		#endregion IAppearanceObserver

		public static readonly Color DefaultBackgroundColor = Color.FromRgb(33, 150, 243);
		public static readonly Color DefaultForegroundColor = Colors.White;
		public static readonly Color DefaultTitleColor = Colors.White;
		public static readonly Color DefaultUnselectedColor = Color.FromRgba(255, 255, 255, 180);

		bool _disposed;
		IShellFlyoutRenderer _flyoutRenderer;
		FrameLayout _frameLayout;

		event EventHandler<VisualElementChangedEventArgs> _elementChanged;
		event EventHandler<PropertyChangedEventArgs> _elementPropertyChanged;

		public ShellRenderer(Context context)
		{
			AndroidContext = context;
		}



		protected Context AndroidContext { get; }
		protected Shell Element { get; private set; }
		FragmentManager FragmentManager => AndroidContext.GetFragmentManager();

		protected virtual IShellObservableFragment CreateFragmentForPage(Page page)
		{
			return new ShellContentFragment(this, page);
		}

		protected virtual IShellFlyoutContentRenderer CreateShellFlyoutContentRenderer()
		{
			return new ShellFlyoutTemplatedContentRenderer(this);
			//return new ShellFlyoutContentRenderer(this, AndroidContext);
		}

		protected virtual IShellFlyoutRenderer CreateShellFlyoutRenderer()
		{
			return new ShellFlyoutRenderer(this, AndroidContext);
		}

		protected virtual IShellItemRenderer CreateShellItemRenderer(ShellItem shellItem)
		{
			return new ShellItemRenderer(this);
		}

		protected virtual IShellSectionRenderer CreateShellSectionRenderer(ShellSection shellSection)
		{
			return new ShellSectionRenderer(this);
		}

		protected virtual IShellToolbarTracker CreateTrackerForToolbar(Toolbar toolbar)
		{
			return new ShellToolbarTracker(this, toolbar, ((IShellContext)this).CurrentDrawerLayout);
		}

		protected virtual IShellToolbarAppearanceTracker CreateToolbarAppearanceTracker()
		{
			return new ShellToolbarAppearanceTracker(this);
		}

		protected virtual IShellTabLayoutAppearanceTracker CreateTabLayoutAppearanceTracker(ShellSection shellSection)
		{
			return new ShellTabLayoutAppearanceTracker(this);
		}

		protected virtual IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
		{
			return new ShellBottomNavViewAppearanceTracker(this, shellItem);
		}

		protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Profile.FrameBegin();

			if (e.PropertyName == Shell.CurrentItemProperty.PropertyName)
				SwitchFragment(FragmentManager, _frameLayout, Element.CurrentItem);

			_elementPropertyChanged?.Invoke(sender, e);

			Profile.FrameEnd();
		}

		protected virtual void OnElementSet(Shell shell)
		{
			Profile.FrameBegin();

			Profile.FramePartition("Flyout");
			_flyoutRenderer = CreateShellFlyoutRenderer();

			Profile.FramePartition("Frame");
			_frameLayout = new CustomFrameLayout(AndroidContext)
			{
				LayoutParameters = new LP(LP.MatchParent, LP.MatchParent),
				Id = AppCompat.Platform.GenerateViewId(),
			};

			Profile.FramePartition("SetFitsSystemWindows");
			_frameLayout.SetFitsSystemWindows(true);

			Profile.FramePartition("AttachFlyout");
			_flyoutRenderer.AttachFlyout(this, _frameLayout);

			Profile.FramePartition("AddAppearanceObserver");
			((IShellController)shell).AddAppearanceObserver(this, shell);

			// Previewer Hack
			Profile.FramePartition("Previewer Hack");
			if (AndroidContext.GetActivity() != null && shell.CurrentItem != null)
				SwitchFragment(FragmentManager, _frameLayout, shell.CurrentItem, false);

			Profile.FrameEnd();
		}

		IShellItemRenderer _currentRenderer;

		protected virtual void SwitchFragment(FragmentManager manager, AView targetView, ShellItem newItem, bool animate = true)
		{
			Profile.FrameBegin();
			Profile.FramePartition("CreateShellItemRenderer");
			var previousRenderer = _currentRenderer;
			_currentRenderer = CreateShellItemRenderer(newItem);
			_currentRenderer.ShellItem = newItem;
			var fragment = _currentRenderer.Fragment;

			Profile.FramePartition("Transaction");
			FragmentTransaction transaction = manager.BeginTransactionEx();

			if (animate)
				transaction.SetTransitionEx((int)global::Android.App.FragmentTransit.FragmentOpen);

			transaction.ReplaceEx(_frameLayout.Id, fragment);
			transaction.CommitAllowingStateLossEx();

			Profile.FramePartition("OnDestroyed");
			void OnDestroyed(object sender, EventArgs args)
			{
				previousRenderer.Destroyed -= OnDestroyed;

				previousRenderer.Dispose();
				previousRenderer = null;
			}

			if (previousRenderer != null)
				previousRenderer.Destroyed += OnDestroyed;

			Profile.FrameEnd();
		}

		void OnElementSizeChanged(object sender, EventArgs e)
		{
			Profile.FrameBegin();

			Profile.FramePartition("ToPixels");
			int width = (int)AndroidContext.ToPixels(Element.Width);
			int height = (int)AndroidContext.ToPixels(Element.Height);

			Profile.FramePartition("Measure");
			_flyoutRenderer.AndroidView.Measure(MeasureSpecFactory.MakeMeasureSpec(width, MeasureSpecMode.Exactly),
				MeasureSpecFactory.MakeMeasureSpec(height, MeasureSpecMode.Exactly));

			Profile.FramePartition("Layout");
			_flyoutRenderer.AndroidView.Layout(0, 0, width, height);

			Profile.FrameEnd();
		}

		void UpdateStatusBarColor(ShellAppearance appearance)
		{
			Profile.FrameBegin("UpdtStatBarClr");

			var activity = AndroidContext.GetActivity();
			var window = activity?.Window;
			var decorView = window?.DecorView;
			var resources = AndroidContext.Resources;

			int statusBarHeight = 0;
			int resourceId = resources.GetIdentifier("status_bar_height", "dimen", "android");
			if (resourceId > 0)
			{
				statusBarHeight = resources.GetDimensionPixelSize(resourceId);
			}

			int navigationBarHeight = 0;
			resourceId = resources.GetIdentifier("navigation_bar_height", "dimen", "android");
			if (resourceId > 0)
			{
				navigationBarHeight = resources.GetDimensionPixelSize(resourceId);
			}

			// TODO Previewer Hack
			if (decorView != null)
			{
				// we are using the split drawable here to avoid GPU overdraw.
				// All it really is is a drawable that only draws under the statusbar/bottom bar to make sure
				// we dont draw over areas we dont need to. This has very limited benefits considering its
				// only saving us a flat color fill BUT it helps people not freak out about overdraw.
				AColor color;
				if (appearance != null)
				{
					color = appearance.BackgroundColor.ToAndroid(Color.FromArgb("#03A9F4"));
				}
				else
				{
					color = Color.FromArgb("#03A9F4").ToAndroid();
				}

				if (!(decorView.Background is SplitDrawable splitDrawable) ||
					splitDrawable.Color != color || splitDrawable.TopSize != statusBarHeight || splitDrawable.BottomSize != navigationBarHeight)
				{
					Profile.FramePartition("Create SplitDrawable");
					var split = new SplitDrawable(color, statusBarHeight, navigationBarHeight);
					Profile.FramePartition("SetBackground");
					decorView.SetBackground(split);
				}
			}

			Profile.FrameEnd("UpdtStatBarClr");
		}

		class SplitDrawable : Drawable
		{
			public int BottomSize { get; }
			public AColor Color { get; }
			public int TopSize { get; }

			public SplitDrawable(AColor color, int topSize, int bottomSize)
			{
				Color = color;
				BottomSize = bottomSize;
				TopSize = topSize;
			}

			public override int Opacity => (int)Format.Opaque;

			public override void Draw(Canvas canvas)
			{
				var bounds = Bounds;

				using (var paint = new Paint())
				{

					paint.Color = Color;

					canvas.DrawRect(new ARect(0, 0, bounds.Right, TopSize), paint);

					canvas.DrawRect(new ARect(0, bounds.Bottom - BottomSize, bounds.Right, bounds.Bottom), paint);

					paint.Dispose();
				}
			}

			public override void SetAlpha(int alpha)
			{
			}

			public override void SetColorFilter(ColorFilter colorFilter)
			{
			}
		}

		#region IDisposable

		void IDisposable.Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing)
			{
				if (_currentRenderer != null && _currentRenderer.Fragment.IsAlive())
				{
					FragmentTransaction transaction = FragmentManager.BeginTransactionEx();
					transaction.RemoveEx(_currentRenderer.Fragment);
					transaction.CommitAllowingStateLossEx();
					FragmentManager.ExecutePendingTransactionsEx();
				}

				Element.PropertyChanged -= OnElementPropertyChanged;
				Element.SizeChanged -= OnElementSizeChanged;
				((IShellController)Element).RemoveAppearanceObserver(this);

				// This cast is necessary because IShellFlyoutRenderer doesn't implement IDisposable
				(_flyoutRenderer as IDisposable)?.Dispose();

				_currentRenderer.Dispose();
				_currentRenderer = null;
			}

			Element = null;
			// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
			// TODO: set large fields to null.

			_disposed = true;
		}

		#endregion IDisposable
	}
}