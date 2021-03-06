﻿using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Gms.Vision;
using Android.Gms.Vision.Texts;
using Android.Util;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V4.App;
using Android;
using Android.Content.PM;
using static Android.Gms.Vision.Detector;
using System.Text;
using System;

namespace TextRecognizerByCamera
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity :Activity, ISurfaceHolderCallback, IProcessor
    {
        private SurfaceView cameraView;
        private TextView txtView;
        private CameraSource cameraSource;
        private
        const int RequestCameraPermissionID = 1001;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            cameraView = FindViewById<SurfaceView>(Resource.Id.surface_view);
            txtView = FindViewById<TextView>(Resource.Id.txtview);
           

            TextRecognizer txtRecognizer = new TextRecognizer.Builder(ApplicationContext).Build();
            if (!txtRecognizer.IsOperational)
            {
                Log.Error("Main Activity", "Detector dependencies are not yet available");
            }
            else
            {
                cameraSource = new CameraSource.Builder(ApplicationContext, txtRecognizer)
                    .SetFacing(CameraFacing.Back)
                    .SetRequestedPreviewSize(1280, 1024)
                    .SetRequestedFps(2.0f)
                    .SetAutoFocusEnabled(true)
                    .Build();
                cameraView.Holder.AddCallback(this);
                txtRecognizer.SetProcessor(this);
            }

          

        }

       

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height) { }
        public void SurfaceCreated(ISurfaceHolder holder)
        {
            if (ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
            {
                //Request permission  
                ActivityCompat.RequestPermissions(this, new string[] {
                    Android.Manifest.Permission.Camera
                }, RequestCameraPermissionID);
                return;
            }
            cameraSource.Start(cameraView.Holder);
        }
        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            cameraSource.Stop();
            
        }
        public void ReceiveDetections(Detections detections)
        {
            SparseArray items = detections.DetectedItems;
            if (items.Size() != 0)
            {
                txtView.Post(() => {
                    StringBuilder strBuilder = new StringBuilder();
                    for (int i = 0; i < items.Size(); ++i)
                    {
                        strBuilder.Append(((TextBlock)items.ValueAt(i)).Value);
                        strBuilder.Append("\n");
                    }
                    txtView.Text = strBuilder.ToString();
                });
            }
        }
        public void Release() { }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestCameraPermissionID:
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            cameraSource.Start(cameraView.Holder);
                        }
                    }
                    break;
            }
        }
    }
}