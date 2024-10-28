//
//  UIViewController+Presentation.h
//  Native Plugins
//
//  Created by Ashwin kumar on 22/01/19.
//  Copyright (c) 2019 Voxel Busters Interactive LLP. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface NSError (Utility)

+ (NSError*)createWithDomain:(NSString*) domain
                withCode:(int) code
         withDescription:(NSString*) description;

@end
