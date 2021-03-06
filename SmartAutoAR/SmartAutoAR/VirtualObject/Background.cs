﻿using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using SmartAutoAR.VirtualObject.Base;
using Bitmap = System.Drawing.Bitmap;

namespace SmartAutoAR.VirtualObject
{
	/// <summary>
	/// 用於渲染背景的類別
	/// </summary>
	public class Background : IDisposable
	{
		protected static float[] vertices =
		{
			//Position          Texture coordinates
			 1.0f,  1.0f, 0.0f, 1.0f, 0.0f, // top right
			 1.0f, -1.0f, 0.0f, 1.0f, 1.0f, // bottom right
			-1.0f, -1.0f, 0.0f, 0.0f, 1.0f, // bottom left
			-1.0f,  1.0f, 0.0f, 0.0f, 0.0f  // top left
		};

		protected static uint[] indices =
		{
			0, 1, 3,
			1, 2, 3
		};

		protected readonly int VAO, VBO, EBO;
		protected Shader shader;
		protected Texture texture;

		public Background()
		{
			// 生成
			VAO = GL.GenVertexArray();
			VBO = GL.GenBuffer();
			EBO = GL.GenBuffer();
			shader = Shader.BackgroundShader;
			texture = new Texture();

			// 設定 VBO 規格
			GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
			GL.NamedBufferStorage(VBO, sizeof(float) * vertices.Length, vertices, BufferStorageFlags.MapWriteBit);

			GL.BindVertexArray(VAO);
			// 設定 VAO 屬性 0 (position)
			GL.VertexArrayAttribBinding(VAO, 0, 0);
			GL.EnableVertexArrayAttrib(VAO, 0);
			GL.VertexArrayAttribFormat(VAO, 0, 3, VertexAttribType.Float, false, 0);

			// 設定 VAO 屬性 1 (textureCoord)
			GL.VertexArrayAttribBinding(VAO, 1, 0);
			GL.EnableVertexArrayAttrib(VAO, 1);
			GL.VertexArrayAttribFormat(VAO, 1, 2, VertexAttribType.Float, false, 12);

			// 將 VAO 與 VBO 串起來
			GL.VertexArrayVertexBuffer(VAO, 0, VBO, IntPtr.Zero, sizeof(float) * 5);

			// 設定 EBO
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
			GL.NamedBufferStorage(EBO, indices.Length * sizeof(uint), indices, BufferStorageFlags.MapWriteBit);
		}

		/// <summary>
		/// 設定欲選染於背景的圖片
		/// </summary>
		/// <param name="frame"></param>
		public void SetImage(Bitmap frame)
		{
			texture.SetImage(frame);
		}

		/// <summary>
		/// 在畫面上渲染背景
		/// </summary>
		public void Render()
		{
			GL.Disable(EnableCap.DepthTest);
			this.shader.Use();
			texture.Use(TextureUnit.Texture0);
			GL.BindVertexArray(VAO);
			GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
			GL.Enable(EnableCap.DepthTest);
		}

		/// <summary>
		/// 釋放資源
		/// </summary>
		public void Dispose()
		{
			GL.DeleteVertexArray(VAO);
			GL.DeleteBuffer(VBO);
			GL.DeleteBuffer(EBO);
			shader.Dispose();
			texture.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
