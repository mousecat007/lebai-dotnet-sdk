﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Robotc;
using Empty = Google.Protobuf.WellKnownTypes.Empty;

namespace Lebai.SDK
{
   public partial class LebaiRobotClient
   {
      public static readonly Dictionary<string, LebaiRobotClient> LebaiRobotClients = new();

      private readonly HttpClient _httpClient;

      private RobotController.RobotControllerClient _robotControllerClient;
      private RobotPrivateController.RobotPrivateControllerClient _robotPrivateControllerClient;

      public LebaiRobotClient(string ip, int robotControllerClientPort = 5181, int robotPrivateControllerPort = 5182,
         GrpcChannelOptions grpcChannelOptions = null)
      {
         IP = ip;
         RobotControllerClientPort = robotControllerClientPort;
         RobotPrivateControllerPort = robotPrivateControllerPort;
         GrpcChannelOptions = grpcChannelOptions;
         _httpClient = new HttpClient {BaseAddress = new Uri($"http://{ip}")};
      }

      private string IP { get; }
      private int RobotControllerClientPort { get; }
      private int RobotPrivateControllerPort { get; }
      private GrpcChannelOptions GrpcChannelOptions { get; }

      public Action<RpcException> OnRpcException { get; set; }

      private RobotController.RobotControllerClient RobotControllerClient =>
         LazyInitializer.EnsureInitialized(ref _robotControllerClient,
            () =>
            {
               var r = new RobotController.RobotControllerClient(
                  GrpcChannel.ForAddress(new Uri($"http://{IP}:{RobotControllerClientPort}"),
                     GrpcChannelOptions ??
                     new GrpcChannelOptions
                     {
                        MaxRetryAttempts = 5
                     })
               );
               return r;
            });

      private RobotPrivateController.RobotPrivateControllerClient RobotPrivateControllerClient =>
         LazyInitializer.EnsureInitialized(ref _robotPrivateControllerClient,
            () => new RobotPrivateController.RobotPrivateControllerClient(
               GrpcChannel.ForAddress(new Uri($"http://{IP}:{RobotPrivateControllerPort}"), GrpcChannelOptions ??
                  new GrpcChannelOptions
                  {
                     MaxRetryAttempts = 5
                  })));

      public static LebaiRobotClient Get(string ip, int robotControllerClientPort = 5181,
         int robotPrivateControllerPort = 5182)
      {
         if (LebaiRobotClients.ContainsKey(ip)) return LebaiRobotClients[ip];

         LebaiRobotClients[ip] = new LebaiRobotClient(ip);
         return LebaiRobotClients[ip];
      }

      /// <summary>
      ///    启用/禁用 设备
      /// </summary>
      /// <param name="externalIoState"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task ConnectExternalIO(ExternalIOState externalIoState,
         CancellationToken cancellationToken = default)
      {
         await RobotPrivateControllerClient.ConnectExternalIOAsync(externalIoState,
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    关闭电源
      /// </summary>
      public virtual async Task PowerDown(CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.PowerDownAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    等待，单位毫秒
      /// </summary>
      /// <param name="sleepRequest"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task Sleep(SleepRequest sleepRequest, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SleepAsync(sleepRequest, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    同步，等待命令执行完成
      /// </summary>
      public virtual async Task Sync(CancellationToken cancellationToken = default)
      {
         try
         {
            await RobotControllerClient.SyncAsync(new Empty(), cancellationToken: cancellationToken);
         }
         catch (RpcException e)
         {
            OnRpcException?.Invoke(e);
         }
      }

      public virtual async Task SyncFor(SyncRequest syncRequest, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SyncForAsync(syncRequest, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    开启示教模式
      /// </summary>
      public virtual async Task TeachMode(CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.TeachModeAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    关闭示教模式
      /// </summary>
      public virtual async Task EndTeachMode(CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.EndTeachModeAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置速度因子（0-100）
      /// </summary>
      /// <param name="factor"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetVelocityFactor(Factor factor, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetVelocityFactorAsync(factor, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取速度因子（0-100）
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Factor> GetVelocityFactor(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetVelocityFactorAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置重力方向
      /// </summary>
      /// <param name="coordinate"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetGravity(Coordinate coordinate, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetGravityAsync(coordinate, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取重力方向
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Coordinate> GetGravity(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetGravityAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置负载
      /// </summary>
      /// <param name="payload"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetPayload(Payload payload, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetPayloadAsync(payload, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取负载
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Payload> GetPayload(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetPayloadAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置负责质量
      /// </summary>
      /// <param name="payloadMass"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetPayloadMass(PayloadMass payloadMass,
         CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetPayloadMassAsync(payloadMass, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取负责质量
      /// </summary>
      /// <returns></returns>
      public virtual async Task<PayloadMass> GetPayloadMass(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetPayloadMassAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置负载质心
      /// </summary>
      /// <param name="payloadCog"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetPayloadCog(PayloadCog payloadCog, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetPayloadCogAsync(payloadCog, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取负责质心
      /// </summary>
      /// <returns></returns>
      public virtual async Task<PayloadCog> GetPayloadCog(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetPayloadCogAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置TCP
      /// </summary>
      /// <param name="pR"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetTcp(PR pR, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetTcpAsync(pR, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取TCP
      /// </summary>
      /// <returns></returns>
      public virtual async Task<PR> GetTcp(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetTcpAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置手爪幅度：0-100 double
      /// </summary>
      /// <param name="amplitude"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetClawAmplitude(Amplitude amplitude,
         CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetClawAmplitudeAsync(amplitude, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得手爪幅度：0-100 double
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Amplitude> GetClawAmplitude(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetClawAmplitudeAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得手爪目前是否夹紧物体状态1表示夹紧，0为松开
      /// </summary>
      /// <returns></returns>
      public virtual async Task<HoldOn> GetClawHoldOn(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetClawHoldOnAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置手爪力度：0-100 double
      /// </summary>
      /// <param name="force"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetClawForce(Force force, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetClawForceAsync(force, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得手爪称重结果
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Weight> GetClawWeight(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetClawWeightAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    implement later
      /// </summary>
      /// <returns></returns>
      public virtual async Task<ForceTorque> GetTcpForce(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetTcpForceAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置手爪
      /// </summary>
      /// <param name="clawInfo"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<ClawInfo> SetClaw(ClawInfo clawInfo, CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.SetClawAsync(clawInfo, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取手爪
      /// </summary>
      /// <returns></returns>
      public virtual async Task<ClawInfo> GetClaw(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetClawAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置位置
      /// </summary>
      /// <param name="jPose"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetPos(JPose jPose, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetPosAsync(jPose, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    implement later
      /// </summary>
      /// <param name="speedJRequest"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SpeedJ(SpeedJRequest speedJRequest, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SpeedJAsync(speedJRequest, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    implement later
      /// </summary>
      /// <param name="speedLRequest"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SpeedL(SpeedLRequest speedLRequest, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SpeedLAsync(speedLRequest, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    implement later
      /// </summary>
      /// <param name="stopJRequest"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task StopJ(StopJRequest stopJRequest, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.StopJAsync(stopJRequest, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    implement later
      /// </summary>
      /// <param name="stopLRequest"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task StopL(StopLRequest stopLRequest, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.StopLAsync(stopLRequest, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    停止当前移动
      /// </summary>
      public virtual async Task StopMove(CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.StopMoveAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    圆弧移动
      /// </summary>
      /// <param name="moveCRequest"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task MoveC(MoveCRequest moveCRequest, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.MoveCAsync(moveCRequest, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    关节空间线性移动
      /// </summary>
      /// <param name="moveJRequest"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task MoveJ(MoveJRequest moveJRequest, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.MoveJAsync(moveJRequest, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    笛卡尔空间线性移动
      /// </summary>
      /// <param name="moveLRequest"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task MoveL(MoveLRequest moveLRequest, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.MoveLAsync(moveLRequest, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    DEPRECIATED
      /// </summary>
      /// <param name="moveLRequest"></param>
      /// <param name="cancellationToken"></param>
      [Obsolete]
      public virtual async Task MoveLJ(MoveLRequest moveLRequest, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.MoveLJAsync(moveLRequest, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    implement later
      /// </summary>
      /// <param name="movePRequest"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task MoveP(MovePRequest movePRequest, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.MovePAsync(movePRequest, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    pt move
      /// </summary>
      /// <param name="pVATRequest"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task MovePT(PVATRequest pVATRequest, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.MovePTAsync(pVATRequest, cancellationToken: cancellationToken);
      }

      /*public void MovePTStream(PVATRequest request,CancellationToken cancellationToken=default)
      {
          
      }*/

      /// <summary>
      ///    pvt move
      /// </summary>
      /// <param name="pVATRequest"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task MovePVT(PVATRequest pVATRequest, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.MovePVTAsync(pVATRequest, cancellationToken: cancellationToken);
      }

      /*public void MovePVTStream(PVATRequest request,CancellationToken cancellationToken=default)
      {
          RobotControllerClient.MovePVTStreamAsync(PVATRequest,cancellationToken:cancellationToken);
      }*/

      /// <summary>
      ///    pvat move
      /// </summary>
      /// <param name="pVATRequest"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task MovePVAT(PVATRequest pVATRequest, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.MovePVATAsync(pVATRequest, cancellationToken: cancellationToken);
      }

      /*public void MovePVATStream(stream stream PVATRequest,CancellationToken cancellationToken=default)
      {
          RobotControllerClient.MovePVATStreamAsync(PVATRequest,cancellationToken:cancellationToken);
      }*/

      /// <summary>
      ///    implement later
      /// </summary>
      /// <param name="servoCRequest"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task ServoC(ServoCRequest servoCRequest, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.ServoCAsync(servoCRequest, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    implement later
      /// </summary>
      /// <param name="servoJRequest"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task ServoJ(ServoJRequest servoJRequest, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.ServoJAsync(servoJRequest, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取机器人所有状态数据
      /// </summary>
      /// <returns></returns>
      public virtual async Task<RobotData> GetRobotData(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetRobotDataAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取机器人状态
      /// </summary>
      /// <returns></returns>
      public virtual async Task<RobotMode> GetRobotMode(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetRobotModeAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得实际关节位置
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Joint> GetActualJointPositions(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetActualJointPositionsAsync(new Empty(),
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得目标关节位置
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Joint> GetTargetJointPositions(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetTargetJointPositionsAsync(new Empty(),
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得实际关节速度
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Joint> GetActualJointSpeeds(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetActualJointSpeedsAsync(new Empty(),
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得目标关节速度
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Joint> GetTargetJointSpeeds(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetTargetJointSpeedsAsync(new Empty(),
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得末端在笛卡尔坐标系下的位姿
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Vector> GetActualTcpPose(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetActualTcpPoseAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得末端在笛卡尔坐标系下的目标位姿
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Vector> GetTargetTcpPose(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetTargetTcpPoseAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    implement later
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Vector> GetActualTcpSpeed(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetActualTcpSpeedAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    implement later
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Vector> GetTargetTcpSpeed(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetTargetTcpSpeedAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    implement later
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Vector> GetActualFlangePose(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetActualFlangePoseAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取关节扭矩
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Joint> GetJointTorques(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetJointTorquesAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取控制器温度
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Temperature> GetControllerTemp(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetControllerTempAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取关节内部温度
      /// </summary>
      /// <param name="intRequest"></param>
      /// <returns></returns>
      public virtual async Task<Temperature> GetJointTemp(IntRequest intRequest,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetJointTempAsync(intRequest, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    implement later
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Current> GetToolCurrent(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetToolCurrentAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置数字输出端口的值
      /// </summary>
      /// <param name="dIO"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetDIO(DIO dIO, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetDIOAsync(dIO, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置扩展数字输出端口的值
      /// </summary>
      /// <param name="dIO"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetExtraDIO(DIO dIO, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetExtraDIOAsync(dIO, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得数字输入端口的值
      /// </summary>
      /// <param name="iOPin"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<DIO> GetDIO(IOPin iOPin, CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetDIOAsync(iOPin, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得扩展数字数如端口的值
      /// </summary>
      /// <param name="iOPin"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<DIO> GetExtraDIO(IOPin iOPin, CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetExtraDIOAsync(iOPin, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置TCP数字输出端口的值
      /// </summary>
      /// <param name="dIO"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetTcpDIO(DIO dIO, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetTcpDIOAsync(dIO, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得TCP数字输入端口的值
      /// </summary>
      /// <param name="iOPin"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<DIO> GetTcpDIO(IOPin iOPin, CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetTcpDIOAsync(iOPin, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置模拟输出端口的值
      /// </summary>
      /// <param name="aIO"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetAIO(AIO aIO, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetAIOAsync(aIO, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得模拟输入端口的值
      /// </summary>
      /// <param name="aIO"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetExtraAIO(AIO aIO, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetExtraAIOAsync(aIO, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得模拟输入端口的值
      /// </summary>
      /// <param name="iOPin"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<AIO> GetAIO(IOPin iOPin, CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetAIOAsync(iOPin, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得扩展模拟输入端口的值
      /// </summary>
      /// <param name="iOPin"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<AIO> GetExtraAIO(IOPin iOPin, CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetExtraAIOAsync(iOPin, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置模拟输入端口工作模式：0:电压，1:电流
      /// </summary>
      /// <param name="aIO"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetAInMode(AIO aIO, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetAInModeAsync(aIO, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置扩展模拟输入端口工作模式：0:电压，1:电流
      /// </summary>
      /// <param name="aIO"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetExtraAInMode(AIO aIO, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetExtraAInModeAsync(aIO, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得模拟输入端口工作模式：0:电压，1:电流
      /// </summary>
      /// <param name="iOPin"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<AIO> GetAInMode(IOPin iOPin, CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetAInModeAsync(iOPin, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得扩展模拟输入端口工作模式：0:电压，1:电流
      /// </summary>
      /// <param name="iOPin"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<AIO> GetExtraAInMode(IOPin iOPin, CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetExtraAInModeAsync(iOPin, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置模拟输出端口工作模式：0:电压，1:电流
      /// </summary>
      /// <param name="aIO"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetAOutMode(AIO aIO, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetAOutModeAsync(aIO, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置扩展模拟输出端口工作模式：0:电压，1:电流
      /// </summary>
      /// <param name="aIO"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetExtraAOutMode(AIO aIO, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetExtraAOutModeAsync(aIO, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得模拟输出端口工作模式：0:电压，1:电流
      /// </summary>
      /// <param name="iOPin"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<AIO> GetAOutMode(IOPin iOPin, CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetAOutModeAsync(iOPin, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得扩展模拟输出端口工作模式：0:电压，1:电流
      /// </summary>
      /// <param name="iOPin"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<AIO> GetExtraAOutMode(IOPin iOPin, CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetExtraAOutModeAsync(iOPin, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    开启/启动系统
      /// </summary>
      public virtual async Task StartSys(CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.StartSysAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    关闭/停止系统
      /// </summary>
      public virtual async Task StopSys(CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.StopSysAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    程序停止
      /// </summary>
      public virtual async Task Stop(CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.StopAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    急停
      /// </summary>
      public virtual async Task EStop(CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.EStopAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取kdl参数
      /// </summary>
      /// <returns></returns>
      public virtual async Task<KDParam> GetKDL(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetKDLAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    查询系统里面的日志信息
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Logs> GetLogs(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetLogsAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获得当前正在执行的命令id，如果没有在执行的命令，则返回-1
      /// </summary>
      public virtual async Task GetCurrentCmd(CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.GetCurrentCmdAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /*
      // 获得指定命令id的执行结果：-1: 未执行；0: 已执行
      public CmdStatus GetCmdExecStatus(void cmdId,CancellationToken cancellationToken=default)
      {
          return await nt.GetCmdExecStatus(cmdId,cancellationToken:cancellationToken);
      }*/

      /// <summary>
      ///    开始微调: 如果当前有其他微调再传入新的微调命令会终止当前的微调进行新的微调
      /// </summary>
      /// <param name="fineTuning"></param>
      public virtual async Task StartFineTuning(FineTuning fineTuning,
         CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.StartFineTuningAsync(fineTuning, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    停止微调
      /// </summary>
      public virtual async Task StopFineTuning(CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.StopFineTuningAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    暂停机器人
      /// </summary>
      public virtual async Task Pause(CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.PauseAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    恢复机器人
      /// </summary>
      public virtual async Task Resume(CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.ResumeAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    机器人正解
      /// </summary>
      /// <param name="joint"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Vector> KinematicsForward(Joint joint,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.KinematicsForwardAsync(joint, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    机器人反解
      /// </summary>
      /// <param name="vector"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Joint> KinematicsInverse(Vector vector,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.KinematicsInverseAsync(vector, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    TCP示教添加
      /// </summary>
      /// <param name="calcTcpParam"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Vector> CalcTcpTranslation(CalcTcpParam calcTcpParam,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.CalcTcpTranslationAsync(calcTcpParam, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    测试命令，以给定的RPY数据执行线性移动
      /// </summary>
      /// <param name="moveLRPYRequest"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task MoveLRPY(MoveLRPYRequest moveLRPYRequest,
         CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.MoveLRPYAsync(moveLRPYRequest, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置LED灯状态
      /// </summary>
      /// <param name="lEDStatus"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetLED(LEDStatus lEDStatus, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetLEDAsync(lEDStatus, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置声音
      /// </summary>
      /// <param name="voiceStatus"></param>
      public virtual async Task SetVoice(VoiceStatus voiceStatus, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetVoiceAsync(voiceStatus, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置风扇
      /// </summary>
      /// <param name="fanStatus"></param>
      /// <param name="cancellationToken"></param>
      public virtual async Task SetFan(FanStatus fanStatus, CancellationToken cancellationToken = default)
      {
         await RobotControllerClient.SetFanAsync(fanStatus, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取灯板状态
      /// </summary>
      /// <returns></returns>
      public virtual async Task<LampStatus> GetLampStatus(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetLampStatusAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    Lua 状态查询
      /// </summary>
      /// <returns></returns>
      public virtual async Task<LuaStatus> GetLuaState(CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetLuaStateAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置外置数字输出
      /// </summary>
      /// <param name="externalDigital"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Response> SetExternalDO(ExternalDigital externalDigital,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.SetExternalDOAsync(externalDigital, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取外置数字输出
      /// </summary>
      /// <param name="externalPin"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<ExternalDigital> GetExternalDO(ExternalPin externalPin,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetExternalDOAsync(externalPin, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取外置数字输入
      /// </summary>
      /// <param name="externalPin"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<ExternalDigital> GetExternalDI(ExternalPin externalPin,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetExternalDIAsync(externalPin, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置外置模拟输出
      /// </summary>
      /// <param name="externalAnalog"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Response> SetExternalAO(ExternalAnalog externalAnalog,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.SetExternalAOAsync(externalAnalog, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取外置模拟输出
      /// </summary>
      /// <param name="externalPin"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<ExternalAnalog> GetExternalAO(ExternalPin externalPin,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetExternalAOAsync(externalPin, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取外置模拟输入
      /// </summary>
      /// <param name="externalPin"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<ExternalAnalog> GetExternalAI(ExternalPin externalPin,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetExternalAIAsync(externalPin, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取某个外置io的全部io信息
      /// </summary>
      /// <param name="externalDevice"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<ExternalIOs> GetExternalIOs(ExternalDevice externalDevice,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetExternalIOsAsync(externalDevice, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置某个外置io的全部io信息
      /// </summary>
      /// <param name="externalDigitals"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Response> SetExternalDOs(ExternalDigitals externalDigitals,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.SetExternalDOsAsync(externalDigitals, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取外置数字输出
      /// </summary>
      /// <param name="externalPins"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<ExternalDigitals> GetExternalDOs(ExternalPins externalPins,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetExternalDOsAsync(externalPins, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取外置数字输入
      /// </summary>
      /// <param name="externalPins"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<ExternalDigitals> GetExternalDIs(ExternalPins externalPins,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetExternalDIsAsync(externalPins, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置外置模拟输出
      /// </summary>
      /// <param name="externalAnalogs"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Response> SetExternalAOs(ExternalAnalogs externalAnalogs,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.SetExternalAOsAsync(externalAnalogs, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取外置模拟输出
      /// </summary>
      /// <param name="externalPins"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<ExternalAnalogs> GetExternalAOs(ExternalPins externalPins,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetExternalAOsAsync(externalPins, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取外置模拟输入
      /// </summary>
      /// <param name="externalPins"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<ExternalAnalogs> GetExternalAIs(ExternalPins externalPins,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetExternalAIsAsync(externalPins, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置信号量
      /// </summary>
      /// <param name="signalValue"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<SignalResult> SetSignal(SignalValue signalValue,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.SetSignalAsync(signalValue, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取信号量
      /// </summary>
      /// <param name="signalValue"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<SignalResult> GetSignal(SignalValue signalValue,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.GetSignalAsync(signalValue, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    添加信号量
      /// </summary>
      /// <param name="signalValue"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<SignalResult> AddSignal(SignalValue signalValue,
         CancellationToken cancellationToken = default)
      {
         return await RobotControllerClient.AddSignalAsync(signalValue, cancellationToken: cancellationToken);
      }
      /*

      public stream RegisterSignals(stream stream SignalList,CancellationToken cancellationToken=default)
      {
          return await RobotControllerClient.RegisterSignals().RequestStream.WriteAsync(cancellationToken:cancellationToken);
      }*/

      /// <summary>
      ///    初始化配置
      /// </summary>
      /// <param name="configuration"></param>
      /// <param name="cancellationToken"></param>
      public virtual async void Init(Configuration configuration, CancellationToken cancellationToken = default)
      {
         await RobotPrivateControllerClient.InitAsync(configuration, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取机器人基础信息
      /// </summary>
      /// <returns></returns>
      public virtual async Task<RobotInfo> GetRobotInfo(CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.GetRobotInfoAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置机器人安装方向
      /// </summary>
      /// <param name="installDirection"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Response> SetInstallDirection(InstallDirection installDirection,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.SetInstallDirectionAsync(installDirection,
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置碰撞检测
      /// </summary>
      /// <param name="collisionDetector"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Response> SetCollisionDetector(CollisionDetector collisionDetector,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.SetCollisionDetectorAsync(collisionDetector,
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置关节配置
      /// </summary>
      /// <param name="jointConfigs"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Response> SetJointConfig(JointConfigs jointConfigs,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.SetJointConfigAsync(jointConfigs,
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置笛卡尔空间的配置
      /// </summary>
      /// <param name="cartesianConfig"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Response> SetCartesianConfig(CartesianConfig cartesianConfig,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.SetCartesianConfigAsync(cartesianConfig,
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    开启DDS
      /// </summary>
      /// <param name="trueOrFalse"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Response> EnableDDS(TrueOrFalse trueOrFalse,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.EnableDDSAsync(trueOrFalse, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置碰撞检测力矩差阈值
      /// </summary>
      /// <param name="collisionTorqueDiff"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Response> SetCollisionTorqueDiff(CollisionTorqueDiff collisionTorqueDiff,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.SetCollisionTorqueDiffAsync(collisionTorqueDiff,
            cancellationToken: cancellationToken);
      }

      // 注册通知事件
      /*public virtual async stream Notification RegisterNotification(google.protobuf.Empty){
      await RobotPrivateControllerClient.RegisterNotification(new Empty(),cancellationToken:cancellationToken);
      }*/
      public virtual async Task<DriverInfo> RobotDriverInfo(CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.RobotDriverInfoAsync(new Empty(),
            cancellationToken: cancellationToken);
      }

      /*// 机器人OTA单个设备更新接口
      public virtual async stream OTAResult RobotOTA(OTAData){
      await RobotPrivateControllerClient.RobotOTAAsync(cancellationToken:cancellationToken);
      }*/

      /// <summary>
      ///    通知灯板、法兰、关节切换分区
      /// </summary>
      /// <param name="otaCmd"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Response> SwitchOtaPartition(OTACmd otaCmd,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.SwitchOtaPartitionAsync(otaCmd,
            cancellationToken: cancellationToken);
      }

      /*// 机器人OTA批量更新接口
      public virtual async stream OTAResults RobotOTABatch(OTADatas){
      await RobotPrivateControllerClient.RobotOTABatchAsync(cancellationToken:cancellationToken);
      }*/

      /// <summary>
      ///    重置
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Response> Reset(CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.ResetAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    以给定角度置零
      /// </summary>
      /// <param name="zero"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Response> InitZero(Zero zero, CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.InitZeroAsync(zero, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    以零位置零
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Response> SetZero(CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.SetZeroAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取机器人电压V
      /// </summary>
      /// <returns></returns>
      public virtual async Task<CurrentVoltage> GetVoltage(CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.GetVoltageAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置单关节伺服参数
      /// </summary>
      /// <param name="jointServoParam"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<JointServoParams> SetServoParam(JointServoParam jointServoParam,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.SetServoParamAsync(jointServoParam,
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取当前所有关节伺服参数
      /// </summary>
      /// <returns></returns>
      public virtual async Task<JointServoParams> GetServoParams(CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.GetServoParamsAsync(new Empty(),
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    调试设置
      /// </summary>
      /// <param name="debugParams"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<DebugParams> SetDebugParams(DebugParams debugParams,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.SetDebugParamsAsync(debugParams,
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    更改DH参数（三轴平行6参数）
      /// </summary>
      /// <param name="fixDhRequest"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<FixDHResult> FixDHParams(FixDHRequest fixDhRequest,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.FixDHParamsAsync(fixDhRequest, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置LED样式
      /// </summary>
      /// <param name="ledStyle"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<LEDStyles> SetLEDStyle(LEDStyle ledStyle,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.SetLEDStyleAsync(ledStyle, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取LED样式
      /// </summary>
      /// <returns></returns>
      public virtual async Task<LEDStyles> GetLEDStyles(CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.GetLEDStylesAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /*// 注册命令状态事件
      public virtual async stream LuaEvent RegisterLuaEvent(){
      await RobotPrivateControllerClient.RegisterLuaEventAsync(new Empty,cancellationToken:cancellationToken);
      }*/

      /// <summary>
      ///    当推送 ALERT/CONFIRM/INPUT/SELECT，用户在前端确定后调用该接口
      /// </summary>
      /// <param name="confirmInput"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Response> ConfirmCallback(ConfirmInput confirmInput,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.ConfirmCallbackAsync(confirmInput,
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    获取 Lua 上次执行到的机器人位置
      /// </summary>
      /// <returns></returns>
      public virtual async Task<PoseRes> GetLastPose(CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.GetLastPoseAsync(new Empty(), cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    配置Modbus外部IO设备
      /// </summary>
      /// <param name="modbusExternalIOs"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Response> SetModbusExternalIO(ModbusExternalIOs modbusExternalIOs,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.SetModbusExternalIOAsync(modbusExternalIOs,
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    修改按钮配置
      /// </summary>
      /// <param name="buttonConfig"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Response> SetButtonConfig(ButtonConfig buttonConfig,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.SetButtonConfigAsync(buttonConfig,
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置绑定设备开关, true: 不限制设备绑定； false：限制设备绑定逻辑
      /// </summary>
      /// <param name="trueOrFalse"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Response> SetBreakACup(TrueOrFalse trueOrFalse,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.SetBreakACupAsync(trueOrFalse, cancellationToken: cancellationToken);
      }

      /*// PVAT数据记录接口，用户记录pvat数据
      public virtual async stream RecordPVATResponse RecordPVAT(RecordPVATRequest){
      await RobotPrivateControllerClient.RecordPVATAsync(cancellationToken:cancellationToken);
      }*/

      /// <summary>
      ///    停止记录pvat数据
      /// </summary>
      /// <returns></returns>
      public virtual async Task<Response> StopRecordPVAT(CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.StopRecordPVATAsync(new Empty(),
            cancellationToken: cancellationToken);
      }

      /*// 语音升级
      public virtual async stream VoiceResult,cancellationToken:cancellationToken);//yvoi UpgradeVoiceFile(VoiceFile){
      await RobotPrivateControllerClient.UpgradeVoiceFileAsync(cancellationToken:cancellationToken);
      }*/

      /// <summary>
      ///    获取当前 DH 参数
      /// </summary>
      /// <param name="dhRequest"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<DHParams> GetDHParams(DHRequest dhRequest,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.GetDHParamsAsync(dhRequest, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    设置 DH 参数并返回设置后的结果
      /// </summary>
      /// <param name="dhParams"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<DHParams> SetDHParams(DHParams dhParams,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.SetDHParamsAsync(dhParams, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    写伺服控制参数
      /// </summary>
      /// <param name="param"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<ExtraServoParam> WriteExtraServoParam(ExtraServoParam param,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.WriteExtraServoParamAsync(param,
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    读取伺服控制参数
      /// </summary>
      /// <param name="param"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<ExtraServoParam> ReadExtraServoParam(ExtraServoParam param,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.ReadExtraServoParamAsync(param,
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    写多个伺服控制参数
      /// </summary>
      /// <param name="param"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<ExtraServoParams> WriteExtraServoParams(ExtraServoParam param,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.WriteExtraServoParamsAsync(param,
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    读取多个伺服控制参数
      /// </summary>
      /// <returns></returns>
      public virtual async Task<ExtraServoParams> ReadExtraServoParams(
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.ReadExtraServoParamsAsync(new Empty(),
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    重置伺服控制参数
      /// </summary>
      /// <returns></returns>
      public virtual async Task<ExtraServoParams> ResetExtraServoParams(
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.ResetExtraServoParamsAsync(new Empty(),
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    写“主动消回差”参数
      /// </summary>
      /// <param name="param"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<JointBacklash> WriteJointBacklash(JointBacklash param,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.WriteJointBacklashAsync(param, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    读取“主动消回差”参数
      /// </summary>
      /// <param name="param"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<JointBacklash> ReadJointBacklash(JointBacklash param,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.ReadJointBacklashAsync(param, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    写“主动消回差”参数
      /// </summary>
      /// <param name="param"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<JointBacklashes> WriteJointBacklashes(JointBacklash param,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.WriteJointBacklashesAsync(param,
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    读取多个“主动消回差”参数
      /// </summary>
      /// <returns></returns>
      public virtual async Task<JointBacklashes> ReadJointBacklashes(CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.ReadJointBacklashesAsync(new Empty(),
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    重置“主动消回差”参数
      /// </summary>
      /// <returns></returns>
      public virtual async Task<JointBacklashes> ResetJointBacklashes(CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.ResetJointBacklashesAsync(new Empty(),
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    启用主动消回差
      /// </summary>
      /// <param name="param"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<EnableJointBacklashes> WriteEnableJointBacklashes(EnableJointBacklash param,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.WriteEnableJointBacklashesAsync(param,
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    是否启用主动消回差
      /// </summary>
      /// <returns></returns>
      public virtual async Task<EnableJointBacklashes> ReadEnableJointBacklashes(
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.ReadEnableJointBacklashesAsync(new Empty(),
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    重置主动消回差
      /// </summary>
      /// <returns></returns>
      public virtual async Task<EnableJointBacklashes> ResetEnableJointBacklashes(
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.ResetEnableJointBacklashesAsync(new Empty(),
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    写关节回差参数
      /// </summary>
      /// <param name="param"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<JointBacklashParam> WriteJointBacklashParam(JointBacklashParam param,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.WriteJointBacklashParamAsync(param,
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    读取关节回差参数
      /// </summary>
      /// <param name="param"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<JointBacklashParam> ReadJointBacklashParam(JointBacklashParam param,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.ReadJointBacklashParamAsync(param,
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    写多个关节回差参数
      /// </summary>
      /// <param name="param"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<JointBacklashParams> WriteJointBacklashParams(JointBacklashParam param,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.WriteJointBacklashParamsAsync(param,
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    读多个关节回差参数
      /// </summary>
      /// <returns></returns>
      public virtual async Task<JointBacklashParams> ReadJointBacklashParams(
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.ReadJointBacklashParamsAsync(new Empty(),
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    重置关节回差参数
      /// </summary>
      /// <returns></returns>
      public virtual async Task<JointBacklashParams> ResetJointBacklashParams(
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.ResetJointBacklashParamsAsync(new Empty(),
            cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    启用关节限位检测
      /// </summary>
      /// <param name="param"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Response> EnableJointLimit(TrueOrFalse param,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.EnableJointLimitAsync(param, cancellationToken: cancellationToken);
      }

      /// <summary>
      ///    切换模拟环境
      /// </summary>
      /// <param name="param"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<Response> SwitchSimulate(TrueOrFalse param,
         CancellationToken cancellationToken = default)
      {
         return await RobotPrivateControllerClient.SwitchSimulateAsync(param, cancellationToken: cancellationToken);
      }

#if NET5_0||NET6_0
      /// <summary>
      ///    获取机器人概要数据流
      /// </summary>
      /// <returns></returns>
      public IAsyncEnumerable<RobotBriefData> GetRobotBriefDataStream(CancellationToken cancellationToken = default)
      {
         return RobotControllerClient.GetRobotBriefData().ResponseStream
            .ReadAllAsync(cancellationToken);
      }

      /// <summary>
      ///    获取机器人的IO数据流
      /// </summary>
      /// <returns></returns>
      public virtual IAsyncEnumerable<IO> GetRobotIODataStream(CancellationToken cancellationToken = default)
      {
         return RobotControllerClient.GetRobotIOData().ResponseStream
            .ReadAllAsync(cancellationToken);
      }

      /// <summary>
      ///    获取机器人的IO数据流
      /// </summary>
      /// <returns></returns>
      public virtual AsyncDuplexStreamingCall<RobotDataCmd, IO> GetRobotIOData(
         CancellationToken cancellationToken = default)
      {
         return RobotControllerClient.GetRobotIOData(cancellationToken: cancellationToken);
      }
#endif
      /*// 连接/断开 MODBUS 设备
      public virtual async Task<Response> ConnectExternalIO(ExternalIOState param){
      return await RobotPrivateControllerClient.ConnectExternalIOAsync(param,cancellationToken:cancellationToken);
      }*/
   }
}