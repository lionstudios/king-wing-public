//
//  APVLogger.h
//  AdjustPurchase
//
//  Created by Uglješa Erceg on 24/11/15.
//  Copyright © 2015-Present Adjust GmbH. All rights reserved.
//

#import <Foundation/Foundation.h>

/**
 *  @brief  Possible APVLogger log levels.
 */
typedef enum {
    APVLogLevelDebug = 1,
    APVLogLevelInfo = 2,
    APVLogLevelError = 3,
    APVLogLevelSuppress = 4
} APVLogLevel;

@interface APVLogger : NSObject

+ (id)getInstance;

+ (void)debug:(NSString *)message, ...;

+ (void)info:(NSString *)message, ...;

+ (void)infoPrio:(NSString *)message, ...;

+ (void)error:(NSString *)message, ...;

+ (void)setLogLevel:(APVLogLevel)logLevel;

@end
