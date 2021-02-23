using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.IO;
using System.Linq;

namespace Tests
{
    public class FileGameProgressStorageServiceTests
    {
        [Test]
        public void Should_CreateSaveGameFile_When_SaveIsCalled()
        {
            //Arrange
            int saveSlotIndex = 0;
            var data = DefaultData();
            var sut = BuildSUT();

            //Act
            sut.Save(saveSlotIndex, data);

            //Assert
            var path = sut.GetSaveFilePath(saveSlotIndex);
            Assert.True(File.Exists(path), "Saved file is missing");
        }

        [Test]
        public void Should_GameProgressDataIdentical_When_SaveAndLoad()
        {
            //Arrange
            int saveSlotIndex = 0;
            var data = DefaultData();
            GameProgressData loadData = null;
            var sut = BuildSUT();

            //Act
            sut.Save(saveSlotIndex, data);
            loadData = sut.Load(saveSlotIndex);

            //Assert
            var path = sut.GetSaveFilePath(saveSlotIndex);
            Assert.True(data.uid.IsValidGuid(), "Initial test data is not set");
            Assert.True(data.IsIdentical(loadData), "Saved file is missing");
        }

        /// <summary>
        /// test that all essential game progress data is being saved and loaded
        /// //TODO: can be break down to unit test for specific fields in GameProgressData when more complex scenarios emerge
        /// </summary>
        [Test]
        public void Should_AllGameProgressDataSuccessfulyLoad_When_SaveAndLoad()
        {
            //Arrange
            int saveSlotIndex = 0;
            string uid = System.Guid.NewGuid().ToString();
            var dateTime = System.DateTime.Now;
            var playerPosition = new Vector3(1, 2, 3);
            var playerScaleX = -1;//direction
            //key 1 in slot 2
            var key1ItemUid = StringResources.ObjectIds.Key_01;
            var itemSlot1Uid = StringResources.ObjectIds.ItemSlot1;
            var itemSlot1_ItemUid = key1ItemUid;//Key01 in inventory
            //key 2 in pillar 2
            var pillar2Uid = StringResources.ObjectIds.KeyPillar_02;
            var key2ItemUid = StringResources.ObjectIds.Key_02;

            var data = new GameProgressData();
            data.uid = uid;
            data.createdDate = dateTime.Ticks;
            data.playerPosition = new SerializableVector3(playerPosition);
            data.playerScaleX = playerScaleX;
            data.SaveItemSlot(itemSlot1Uid, itemSlot1_ItemUid);
            data.SaveItem(key1ItemUid, itemSlot1Uid, null);
            data.SaveItem(key2ItemUid, null, pillar2Uid);

            GameProgressData loadData = null;
            var sut = BuildSUT();

            //Act
            sut.Save(saveSlotIndex, data);
            loadData = sut.Load(saveSlotIndex);

            //Assert

            Assert.AreEqual(uid, loadData.uid);
            Assert.AreEqual(dateTime.Ticks, loadData.createdDate);
            Assert.True(playerPosition == loadData.playerPosition, "playerPosition");
            Assert.AreEqual(playerScaleX, data.playerScaleX, "playerScaleX");
            //item slots
            Assert.NotNull(loadData.ItemSlots, "Item slots not saved");
            var loadedSlot1 = loadData.ItemSlots.FirstOrDefault(i => i.uid == itemSlot1Uid);
            Assert.NotNull(loadedSlot1, "Item slots 1 not saved");
            Assert.AreEqual(itemSlot1Uid, loadedSlot1.uid, "Item slot 1 data invalid (Slot Uid)");
            Assert.AreEqual(itemSlot1_ItemUid, loadedSlot1.itemUid, "Item slot 1 data invalid (item Uid)");
            //items
            Assert.NotNull(loadData.Items, "Items not saved");
            //item1 (key1) in slot1
            var loadedKey1 = loadData.Items.FirstOrDefault(i => i.uid == key1ItemUid);
            Assert.NotNull(loadedKey1, $"{nameof(loadedKey1)} not saved");
            Assert.AreEqual(itemSlot1Uid, loadedKey1.slotUid, $"{nameof(loadedKey1)} data invalid (slot Uid)");
            Assert.AreEqual(ItemState.InInventory, loadedKey1.state, $"{nameof(loadedKey1)} data invalid (state)");
            //item 2(key2) inside pillar
            var loadedKey2 = loadData.Items.FirstOrDefault(i => i.uid == key2ItemUid);
            Assert.NotNull(loadedKey2, $"{nameof(loadedKey2)} not saved");
            Assert.AreEqual(pillar2Uid, loadedKey2.dropTargetUid, $"{nameof(loadedKey2)} data invalid (dropTarget Uid)");
            Assert.AreEqual(ItemState.PlacedOnDropTarget, loadedKey2.state, $"{nameof(loadedKey2)} data invalid (state)");
        }

       

        GameProgressData DefaultData()
        {
            var data = new GameProgressData();
            data.uid = System.Guid.NewGuid().ToString();
            data.createdDate = System.DateTime.Now.Ticks;
            return data;
        }

        FileGameProgressStorageService BuildSUT()
        {
            var sut = new FileGameProgressStorageService(true);
            return sut;
        }
    }
}
