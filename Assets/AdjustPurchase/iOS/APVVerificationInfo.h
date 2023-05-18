//
//  APVVerificationInfo.h
//  AdjustPurchase
//
//  Created by Uglješa Erceg on 09/11/15.
//  Copyright © 2015-Present Adjust GmbH. All rights reserved.
//

#import <Foundation/Foundation.h>

/**
 *  @brief  Object which is sent as response from AdjustPurchase module in adjustVerificationUpdate: method.
 */
@interface APVVerificationInfo : NSObject

/**
 *  @property   message
 *
 *  @brief      Text message about current state of receipt verification.
 */
@property (nonatomic, copy) NSString *message;

/**
 *  @property   code
 *
 *  @brief      Response code returned from Adjust backend server.
 */
@property (nonatomic, assign) int code;

/**
 *  @property   verificationStatus
 *
 *  @brief      State of verification (success / failure / unknown / not verified)
 */
@property (nonatomic, copy) NSString *verificationStatus;

/**
 *  @brief  Get verification info as nicely formatted string.
 *
 *  @return Verification info as string.
 */
- (NSString *)getVerificationInfoAsString;

@end
