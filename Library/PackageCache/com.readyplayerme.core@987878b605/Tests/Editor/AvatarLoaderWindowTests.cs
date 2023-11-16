﻿using System.Collections;
using System.Reflection;
using JetBrains.Annotations;
using NUnit.Framework;
using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace ReadyPlayerMe.Core.Tests
{
    public class AvatarLoaderWindowTests
    {
        private GameObject avatar;

        [TearDown]
        public void Cleanup()
        {
            TestUtils.DeleteAvatarDirectoryIfExists(TestAvatarData.DefaultAvatarUri.Guid, true);
            TestUtils.DeleteCachedAvatar(TestAvatarData.DefaultAvatarUri.Guid);
            if (avatar != null)
            {
                Object.DestroyImmediate(avatar);
            }
        }

        [UnityTest]
        public IEnumerator Avatar_Loaded_Stored_And_No_Overrides()
        {
            AvatarLoaderEditor window = EditorWindow.GetWindow<AvatarLoaderEditor>();

            MethodInfo loadAvatarMethod = typeof(AvatarLoaderEditor).GetMethod("LoadAvatar", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsNotNull(loadAvatarMethod);
            loadAvatarMethod.Invoke(window, new object[] { TestAvatarData.DefaultAvatarUri.ModelUrl });

            var time = System.DateTime.Now;

            do
            {
                yield return null;
                avatar = GameObject.Find(TestAvatarData.DefaultAvatarUri.Guid);
            } while (avatar == null && System.DateTime.Now.Subtract(time).Seconds < 5);

            window.Close();
            Assert.IsNotNull(avatar);
            var overrides = PrefabUtility.HasPrefabInstanceAnyOverrides(avatar.gameObject, false);
            Assert.IsFalse(overrides);
        }
    }
}
