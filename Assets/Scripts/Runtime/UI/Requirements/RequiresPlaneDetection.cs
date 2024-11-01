using System.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RequiresPlaneDetection : RequiresARSubsystem<XRPlaneSubsystem, XRPlaneSubsystemDescriptor>
    {
        [SerializeField]
        bool m_RequiresPlaneClassifications;

        protected override IEnumerator Start()
        {
            yield return base.Start();

            if (m_Button.interactable == false)
                yield break;

            var descriptor = s_LoadedSubsystem.subsystemDescriptor;

            if (m_RequiresPlaneClassifications && !descriptor.supportsClassification)
                ARSceneSelectUI.DisableButton(m_Button);
        }
    }
}
