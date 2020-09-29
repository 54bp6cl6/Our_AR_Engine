﻿using OpenTK;
using OpenTK.Graphics;
using SmartAutoAR;
using SmartAutoAR.InputSource;
using SmartAutoAR.VirtualObject;
using SmartAutoAR.VirtualObject.Lights;
using System;
using Bitmap = System.Drawing.Bitmap;

namespace Debug
{
	public partial class ArForm : GameWindow
	{
		IInputSource inputSource;
		ArWorkflow workflow;
		Bitmap marker;
		Scene scene;

		public ArForm(int width, int height, string title) :
			base(width, height,
				GraphicsMode.Default,
				title,
				GameWindowFlags.Default,
				DisplayDevice.Default,
				4, 5,
				GraphicsContextFlags.ForwardCompatible)
		{ }

		protected override void OnLoad(EventArgs e)
		{
			// 設定影像輸入
			//inputSource = new ImageSource(@"background.jpg");
			inputSource = new VideoSource("video.mp4");

			// 建立 workflow 物件
			workflow = new ArWorkflow(inputSource);

			// 導入 marker圖像
			marker = new Bitmap("marker.png");

			// 設定場景
			scene = new Scene();

			// 載入模型
			Model model = Model.LoadModel(@"..\..\..\models\IronMan\IronMan.obj");
			model.Resize(0.005f);
			scene.Models.Add(model);
			scene.Lights.Add(new AmbientLight(Color4.White, 1.0f));
			scene.Lights.Add(new PointLight(Color4.White, new Vector3(0, 10, 10), 1.0f, 0.4f));

			// 設定 marker 對應的 scene
			workflow.MarkerPairs[marker] = scene;

			// 啟用需要的擬真方法
			// LightSourceSimulation = true;
			// ColorTransfer = true;

			base.OnLoad(e);
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			// 確保視窗比例與背景一致
			Width = (int)(Height * workflow.WindowAspectRatio);

			// 對下一幀做處理，包含偵測、渲染、擬真
			//workflow.ShowLast(true);
			workflow.Show();

			// 針對視窗本身做繪製
			SwapBuffers();

			base.OnRenderFrame(e);
		}

		protected override void OnResize(EventArgs e)
		{
			workflow.Resize(Width, Height);

			base.OnResize(e);
		}

		protected override void OnUnload(EventArgs e)
		{
			scene.Dispose();

			base.OnUnload(e);
		}
	}
}
