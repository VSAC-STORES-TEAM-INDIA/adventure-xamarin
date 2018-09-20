using System;

using UIKit;
using SpriteKit;
using Foundation;
using CoreGraphics;

namespace Adventure
{
	public partial class ViewController : UIViewController
	{
		AdventureScene scene;

		public ViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override bool PrefersStatusBarHidden ()
		{
			return true;
		}

		public override bool ShouldAutorotate ()
		{
			return true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Landscape;
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

			// Call BuildScence inside ViewWillLayoutSubviews because in ios7 property View.Bounds
			// always relative to Portrait orientation (but we run game in Landscape)
			// In ios8 you can move BuildScence to ViewDidLoad
			if (scene == null)
				BuildScence ();
		}

		void BuildScence ()
		{
			loadingProgressIndicator.StartAnimating ();

			CGSize size = View.Bounds.Size;
			// On iPhone/iPod touch we want to see a similar amount of the scene as on iPad.
			// So, we set the size of the scene to be double the size of the view, which is
			// the whole screen, 3.5- or 4- inch. This effectively scales the scene to 50%.
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone) {
				size.Height *= 2;
				size.Width *= 2;
			}

			scene = new AdventureScene (size);

			scene.LoadSceneAssetsWithCompletionHandler (() => {
				scene.Initialize();
				scene.ScaleMode = SKSceneScaleMode.AspectFill;
				scene.ConfigureGameControllers();

				loadingProgressIndicator.StopAnimating();
				loadingProgressIndicator.Hidden = true;

				SKView.PresentScene(scene);

				UIView.Animate(2, 0, UIViewAnimationOptions.CurveEaseInOut, ()=> {
					archerButton.Alpha = 1;
					warriorButton.Alpha = 1;
				}, null);
			});

			SKView.ShowsFPS = true;
			SKView.ShowsDrawCount = true;
			SKView.ShowsNodeCount = true;
		}

		void HideUIElements(bool shouldHide, bool shouldAnimate)
		{
			float alpha = shouldHide ? 0.0f : 1.0f;

			Action setAlpha = () => {
				gameLogo.Alpha = alpha;
				archerButton.Alpha = alpha;
				warriorButton.Alpha = alpha;
			};

			if (shouldAnimate)
				UIView.Animate (2, 0, UIViewAnimationOptions.CurveEaseInOut, setAlpha, null);
			else
				setAlpha ();
		}

		[Export("chooseArcher:")]
		void ChooseArcher(NSObject sender)
		{
			StartGameWithHeroType (HeroType.Archer);
		}

		[Export("chooseWarrior:")]
		void ChooseWarrior(NSObject sender)
		{
			StartGameWithHeroType (HeroType.Warrior);
		}

		void StartGameWithHeroType(HeroType type)
		{
			HideUIElements (true, true);
			scene.DefaultPlayerHeroType = type;
			scene.StartLevel ();
		}
	}
}
