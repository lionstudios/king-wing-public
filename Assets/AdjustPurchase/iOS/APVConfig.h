//
//  APVConfig.h
//  AdjustPurchase
//
//  Created by Uglješa Erceg on 07/12/15.
//  Copyright © 2015-Present Adjust GmbH. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "APVLogger.h"

/**
 *  @brief  Environment variables.
 *          We use this environment to distinguish between real traffic and artificial traffic from test devices.
 *          It is very important that you keep this value meaningful at all times!
 *          Please use APVEnvironmentSandbox while TESTING.
 *          Please switch to APVEnvironmentProduction before you RELEASE your app.
 */
extern NSString * const APVEnvironmentSandbox;
extern NSString * const APVEnvironmentProduction;

@interface APVConfig : NSObject

/**
 *  @property   appToken
 *
 *  @brief      The App Token of your app. This unique identifier can
 *              be found it in your dashboard at http://adjust.com and should always
 *              be 12 characters long.
 */
@property (nonatomic, readonly) NSString *appToken;

/**
 *  @property   environment
 *
 *  @brief      The current environment your app. We use this environment to
 *              distinguish between real traffic and artificial traffic from test devices.
 *              It is very important that you keep this value meaningful at all times!
 */
@property (nonatomic, readonly) NSString *environment;

/**
 *  @property   sdkPrefix
 *
 *  @brief      Please, do not use this property on your own!
 *              It is meant for usage from other non-native adjust purchase SDKs.
 *              Default sdk_version for native iOS SDK looks like: ios_purchase1.0.0
 *              By adding prefix in (for example) Unity purchase SDK, value
 *              of sdk_version parameter looks like this: unity1.0.0@ios_purchase1.0.0
 */
@property (nonatomic, copy) NSString *sdkPrefix;

/**
 *  @property   logLevel
 *
 *  @brief      The desired minimum log level (default: debug)
 *              Must be one of the following:
 *              - APVLogLevelDebug     (enable all logs - default)
 *              - APVLogLevelInfo      (disable debug logs)
 *              - APVLogLevelError     (disable info and debug logs)
 *              - APVLogLevelSuppress  (disable all logs)
 */
@property (nonatomic, assign) APVLogLevel logLevel;

/**
 *  @brief  Method used for APVConfig object initialization.
 *
 *  @param  appToken    The App Token of your app. This unique identifier can
 *                      be found it in your dashboard at http://adjust.com and should 
 *                      always be 12 characters long.
 *  @param  environment The current environment your app.
 *                      For more info, please check APVCommon.h file.
 *
 */
- (id)initWithAppToken:(NSString *)appToken
        andEnvironment:(NSString *)environment;

/**
 *  @brief  Method used to check if APVConfig object is valid.
 *
 *  @return Boolean indicating wether APVConfig is valid or not.
 */
- (BOOL)isValid;

@end
