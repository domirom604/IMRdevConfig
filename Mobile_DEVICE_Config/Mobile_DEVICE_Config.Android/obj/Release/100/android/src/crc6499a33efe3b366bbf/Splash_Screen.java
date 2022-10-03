package crc6499a33efe3b366bbf;


public class Splash_Screen
	extends androidx.appcompat.app.AppCompatActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("Mobile_DEVICE_Config.Droid.Splash_Screen, Mobile_DEVICE_Config.Android", Splash_Screen.class, __md_methods);
	}


	public Splash_Screen ()
	{
		super ();
		if (getClass () == Splash_Screen.class)
			mono.android.TypeManager.Activate ("Mobile_DEVICE_Config.Droid.Splash_Screen, Mobile_DEVICE_Config.Android", "", this, new java.lang.Object[] {  });
	}


	public Splash_Screen (int p0)
	{
		super (p0);
		if (getClass () == Splash_Screen.class)
			mono.android.TypeManager.Activate ("Mobile_DEVICE_Config.Droid.Splash_Screen, Mobile_DEVICE_Config.Android", "System.Int32, mscorlib", this, new java.lang.Object[] { p0 });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
