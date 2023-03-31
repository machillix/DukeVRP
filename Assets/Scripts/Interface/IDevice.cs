/******************************************************************************
 * File        : IDevice.cs
 * Version     : 0.9 Alpha
 * Author      : Toni Westerlund (toni.westerlund@lapinamk.com)
 * Copyright   : Lapland University of Applied Sciences
 * Licence     : MIT-Licence
 * 
 * Copyright (c) 2020 Lapland University of Applied Sciences
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace FrostBit
{
    /// <summary>
    /// Device Interface
    /// </summary>
    public interface IDevice
    {

        /// <summary>
        /// Send Data Forward to next Device
        /// </summary>
        /// <param name="aData"></param>
        /// <param name="aSlotId"></param>
        /// <param name="aState"></param>
        void Send(DeviceData aData, int aSlotId, SimulationState aState);

        /// <summary>
        /// Updata Device Data
        /// </summary>
        /// <param name="aData"></param>
        /// <param name="aSlotId"></param>
        void UpdateData(DeviceData aData, int aSlotId);

        /// <summary>
        /// Get Device Datra
        /// </summary>
        /// <returns></returns>
        GenericDeviceData GetDeviceData();

        /// <summary>
        /// Get Slot Out Data
        /// </summary>
        /// <param name="aSlotId"></param>
        /// <returns></returns>
        DeviceData GetSlotOutData(int aSlotId);

        /// <summary>
        /// Get Slotout instance Identifier
        /// </summary>
        /// <param name="aSlotId"></param>
        /// <returns></returns>
        int GetSlotOutInstanceID(int aSlotId);

        /// <summary>
        /// Get SlotIn instance Identifier
        /// </summary>
        /// <param name="aSlotId"></param>
        /// <returns></returns>
        int GetSlotInInstanceID(int aSlotId);

        /// <summary>
        /// Set Parellel
        /// </summary>
        /// <returns></returns>
        GenericDevice GetParellel();

        /// <summary>
        /// Get Device
        /// </summary>
        /// <returns></returns>
        GenericDevice GetDevice();

        /// <summary>
        /// Get Pressure Lost
        /// </summary>
        /// <returns></returns>
        float GetPressureLoss();

        /// <summary>
        /// Set Pressure Lost
        /// </summary>
        /// <param name="aLoss"></param>
        void SetPressureLoss(float aLoss);
    }
}