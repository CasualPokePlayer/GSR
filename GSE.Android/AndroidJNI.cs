// Copyright (c) 2024 CasualPokePlayer
// SPDX-License-Identifier: MPL-2.0

using System;

using GSE.Android.JNI;

namespace GSE.Android;

// ReSharper disable once UnusedType.Global
// ReSharper disable once UnusedMember.Global

/// <summary>
/// Handles Android JNI initialization
/// </summary>
public static unsafe class AndroidJNI
{
	// JNI version 1.4 (what SDL uses)
	private const int JNI_VERSION_1_4 = 0x00010004;
	private const int JNI_OK = 0;

	private static JClass _gseActivityClassId;
	private static bool _nativeFunctionsRegistered;

	public static int Initialize(nint vm)
	{
		var javaVM = (JavaVM*)vm;
		JNIEnv* e;
		var res = javaVM->Vtbl->GetEnv(javaVM, (void**)&e, JNI_VERSION_1_4);
		if (res != JNI_OK)
		{
			Console.Error.WriteLine($"Failed to get JNIEnv* from JavaVM*, this is fatal. Error code given: {JNIException.GetErrorCodeString(res)}");
			return JNI_VERSION_1_4;
		}

		try
		{
			var env = new JNIEnvPtr(e);
			using (var gseActivityClassId =
			       new LocalRefWrapper<JClass>(env, env.FindClass("org/psr/gse/GSEActivity"u8)))
			{
				_gseActivityClassId = (JClass)env.NewGlobalRef(gseActivityClassId.LocalRef);
			}

			AndroidFile.InitializeJNI(env, _gseActivityClassId);
			AndroidCryptography.InitializeJNI(env, _gseActivityClassId);

			fixed (byte*
			       dispatchAndroidKeyEventName = "DispatchAndroidKeyEvent"u8,
			       dispatchAndroidKeyEventSignature = "(IZ)V"u8,
			       setDocumentRequestResultName = "SetDocumentRequestResult"u8,
			       setDocumentRequestResultSignature = "(Ljava/lang/String;)V"u8)
			{
				Span<JNINativeMethod> nativeFunctions = stackalloc JNINativeMethod[2];
				// DispatchAndroidKeyEvent
				nativeFunctions[0].Name = dispatchAndroidKeyEventName;
				nativeFunctions[0].Signature = dispatchAndroidKeyEventSignature;
				nativeFunctions[0].FnPtr = (delegate* unmanaged<JNIEnvPtr, JClass, JInt, JBoolean, void>)&AndroidInput.DispatchAndroidKeyEvent;
				// SetDocumentRequestResult
				nativeFunctions[1].Name = setDocumentRequestResultName;
				nativeFunctions[1].Signature = setDocumentRequestResultSignature;
				nativeFunctions[1].FnPtr = (delegate* unmanaged<JNIEnvPtr, JClass, JString, void>)&AndroidFile.SetDocumentRequestResult;
				env.RegisterNatives(_gseActivityClassId, nativeFunctions);
				_nativeFunctionsRegistered = true;
			}

			return JNI_VERSION_1_4;
		}
		catch (Exception ex)
		{
			Deinitialize(vm);
			// can't show a message box here, as this is a Java thread, not a native thread
			Console.Error.WriteLine($"JNI initialization has failed, this is fatal. Exception given: {ex}");
			return -1;
		}
	}

	public static void Deinitialize(nint vm)
	{
		if (!_gseActivityClassId.IsNull)
		{
			var javaVM = (JavaVM*)vm;
			JNIEnv* e;
			var res = javaVM->Vtbl->GetEnv(javaVM, (void**)&e, JNI_VERSION_1_4);
			if (res != JNI_OK)
			{
				Console.Error.WriteLine($"Failed to get JNIEnv* from JavaVM*, cannot properly deinitialize JNI. Error code given: {JNIException.GetErrorCodeString(res)}");
				return;
			}

			var env = new JNIEnvPtr(e);
			if (_nativeFunctionsRegistered)
			{
				try
				{
					env.UnregisterNatives(_gseActivityClassId);
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine($"Failed to unregister native functions, exception given: {ex}");
				}
			}

			env.DeleteGlobalRef(_gseActivityClassId);
		}
	}
}
