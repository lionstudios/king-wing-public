//
//  APVPurchase.h
//  AdjustPurchase
//
//  Created by Uglješa Erceg on 13/11/15.
//  Copyright © 2015-Present Adjust GmbH. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "APVConfig.h"
#import "APVVerificationInfo.h"

/**
 *  @brief  Static class used for In-App-Purchase receipt verification.
 */
@interface APVPurchase : NSObject

/**
 *  @brief  Initilisation method which needs adjust app token and environment
 *          which are later bundled in HTTP request to adjust backend server.
 *
 *  @param  config  ADJPConfig object.
 */
+ (void)init:(APVConfig *)config;

/**
 *  @brief  Method used to verify In-App-Purchase receipt.
 *
 *  @param  receipt           Apple receipt.
 *  @param  transactionId     SKPaymentTransaction identifier obtained after transaction state became
 *                            SKPaymentTransactionStatePurchased and after transaction has been finished.
 *  @param  productId         Product identifier.
 *  @param  completionHandler Block which will get executed once verification info is available.
 */
+ (void)verifyPurchase:(NSData *)receipt
         transactionId:(NSString *)transactionId
             productId:(NSString *)productId
     completionHandler:(void (^)(APVVerificationInfo *verificationInfo))completionHandler;

@end
