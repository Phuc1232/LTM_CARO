using System;
using System.Collections.Generic;
using System.Text;

namespace caro.share.DTOs.Constants
{
    public enum PacketType
    {
        LoginRequest =1,
        LoginResponse =2,

        ChallengeRequest = 10,      
        ChallengeNotify = 11,       
        ChallengeResponse = 12,     
        ChallengeResult = 13,      
        OnlinePlayerList = 14,      

        ChatSend = 20,             
        ChatReceive = 21,           
   
        TimerUpdate = 30,           
        TimerExpired = 31,          
  
        GameStartNotify = 40,

        MoveRequest = 50,
        MoveNotiFy =51,
        GameEndNotify =52,

        MatchHistoryRequest = 60,
        MatchHistoryResponse = 61
    }
}
