#import "AdjustPurchaseUnity.h"

@implementation AdjustPurchaseUnity

static char* adjustPurchaseSceneName = nil;
static id adjustPurchaseUnityInstance = nil;

- (id)init {
    self = [super init];
    return self;
}

- (void)adjustVerificationUpdate:(APVVerificationInfo *)info {
    NSMutableDictionary *dictVerificationInfo = [[NSMutableDictionary alloc] init];

    [dictVerificationInfo setObject:info.message forKey:@"message"];
    [dictVerificationInfo setObject:info.verificationStatus forKey:@"verificationStatus"];
    [dictVerificationInfo setObject:[NSString stringWithFormat:@"%d", info.code] forKey:@"code"];

    NSData *dataVerificationInfo = [NSJSONSerialization dataWithJSONObject:dictVerificationInfo options:0 error:nil];
    NSString *stringVerificationInfo = [[NSString alloc] initWithBytes:[dataVerificationInfo bytes]
                                                                length:[dataVerificationInfo length]
                                                                encoding:NSUTF8StringEncoding];

    const char* charArrayVerificationInfo = [stringVerificationInfo UTF8String];

    UnitySendMessage(adjustPurchaseSceneName, "GetNativeVerificationInfo", charArrayVerificationInfo);
}

@end

extern "C"
{
    void _AdjustPurchaseInit(const char* appToken, const char* environment, const char* sdkPrefix, int logLevel) {
        NSString *stringSdkPrefix = [NSString stringWithUTF8String:sdkPrefix];
        NSString *stringAppToken = [NSString stringWithUTF8String:appToken];
        NSString *stringEnvironment = [NSString stringWithUTF8String:environment];

        APVConfig *config = [[APVConfig alloc] initWithAppToken:stringAppToken andEnvironment:stringEnvironment];

        if (logLevel != -1) {
            [config setLogLevel:(APVLogLevel)logLevel];
        }
        if (stringSdkPrefix != nil && [stringSdkPrefix length] > 0) {
            [config setSdkPrefix:stringSdkPrefix];
        }

        adjustPurchaseUnityInstance = [[AdjustPurchaseUnity alloc] init];
        [APVPurchase init:config];
    }

    void _AdjustPurchaseVerifyPurchase(const char* receipt, const char* transactionId, const char* productId, const char* sceneName) {
        NSString *stringReceipt = nil;
        NSString *stringTransactionId = nil;
        NSString *stringProducId = nil;
        NSString *stringSceneName = [NSString stringWithUTF8String:sceneName];

        if (sceneName != NULL && [stringSceneName length] > 0) {
            adjustPurchaseSceneName = strdup(sceneName);
        }
        if (receipt != NULL) {
            stringReceipt = [NSString stringWithUTF8String:receipt];
        }
        if (transactionId != NULL) {
            stringTransactionId = [NSString stringWithUTF8String:transactionId];
        }
        if (productId != NULL) {
            stringProducId = [NSString stringWithUTF8String:productId];
        }

        [APVPurchase verifyPurchase:[stringReceipt dataUsingEncoding:NSUTF8StringEncoding]
                      transactionId:stringTransactionId
                          productId:stringProducId
                  completionHandler:^(APVVerificationInfo *verificationInfo) {
            [adjustPurchaseUnityInstance adjustVerificationUpdate:verificationInfo];
        }];
    }
}
