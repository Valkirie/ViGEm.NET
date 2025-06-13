﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using Nefarius.ViGEm.Client.Targets.DualShock4;

namespace Nefarius.ViGEm.Client.Targets;

internal partial class DualShock4Controller
{
    public void SetButtonState(DualShock4Button button, bool pressed)
    {
        switch (button)
        {
            case DualShock4SpecialButton specialButton:
                if (pressed)
                {
                    _nativeReport.bSpecial |= (byte)specialButton.Value;
                }
                else
                {
                    _nativeReport.bSpecial &= (byte)~specialButton.Value;
                }

                break;
            case DualShock4Button normalButton:
                if (pressed)
                {
                    _nativeReport.wButtons |= normalButton.Value;
                }
                else
                {
                    _nativeReport.wButtons &= (ushort)~normalButton.Value;
                }

                break;
        }

        if (AutoSubmitReport)
        {
            SubmitNativeReport(_nativeReport);
        }
    }

    public void SetDPadDirection(DualShock4DPadDirection direction)
    {
        _nativeReport.wButtons &= unchecked((ushort)~0xF);
        _nativeReport.wButtons |= direction.Value;

        if (AutoSubmitReport)
        {
            SubmitNativeReport(_nativeReport);
        }
    }

    public void SetAxisValue(DualShock4Axis axis, byte value)
    {
        switch (axis.Name)
        {
            case "LeftThumbX":
                _nativeReport.bThumbLX = value;
                break;
            case "LeftThumbY":
                _nativeReport.bThumbLY = value;
                break;
            case "RightThumbX":
                _nativeReport.bThumbRX = value;
                break;
            case "RightThumbY":
                _nativeReport.bThumbRY = value;
                break;
        }

        if (AutoSubmitReport)
        {
            SubmitNativeReport(_nativeReport);
        }
    }

    public void SetSliderValue(DualShock4Slider slider, byte value)
    {
        switch (slider.Name)
        {
            case "LeftTrigger":
                _nativeReport.bTriggerL = value;
                break;
            case "RightTrigger":
                _nativeReport.bTriggerR = value;
                break;
        }

        if (AutoSubmitReport)
        {
            SubmitNativeReport(_nativeReport);
        }
    }

    /// <inheritdoc />
    public void SubmitRawReport(byte[] buffer)
    {
        if (buffer.Length != Marshal.SizeOf<ViGEmClient.DS4_REPORT_EX>())
        {
            throw new ArgumentOutOfRangeException(nameof(buffer), "Supplied buffer has invalid size.");
        }

        _nativeReportEx.Report = buffer;

        ViGEmClient.VIGEM_ERROR error =
            ViGEmClient.vigem_target_ds4_update_ex(Client.NativeHandle, NativeHandle, _nativeReportEx);

        ViGEmClient.CheckError(error);
    }

    /// <inheritdoc />
    public IEnumerable<byte> AwaitRawOutputReport()
    {
        ViGEmClient.VIGEM_ERROR error = ViGEmClient.vigem_target_ds4_await_output_report(Client.NativeHandle,
            NativeHandle,
            ref _outputBuffer);

        ViGEmClient.CheckError(error);

        return _outputBuffer.Buffer;
    }

    /// <inheritdoc />
    public IEnumerable<byte> AwaitRawOutputReport(int timeout, out bool timedOut)
    {
        ViGEmClient.VIGEM_ERROR error = ViGEmClient.vigem_target_ds4_await_output_report_timeout(Client.NativeHandle,
            NativeHandle,
            (uint)timeout, ref _outputBuffer);

        if (error == ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_TIMED_OUT)
        {
            timedOut = true;
            return Enumerable.Empty<byte>();
        }

        ViGEmClient.CheckError(error);

        timedOut = false;
        return _outputBuffer.Buffer;
    }
}